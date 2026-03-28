-- Supabase Schema V2 for CAT (Auth-Specific Version)
-- Optimized for Real-Time Attendance Tracking and C# Compatibility
-- This version is designed for use with Supabase Auth.

-- ==========================================
-- 1. Extensions & Cleanup
-- ==========================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Drop existing tables (if any) to ensure a clean slate
DROP TABLE IF EXISTS public.attendance_logs CASCADE;
DROP TABLE IF EXISTS public.enrollments CASCADE;
DROP TABLE IF EXISTS public.sections CASCADE;
DROP TABLE IF EXISTS public.courses CASCADE;
DROP TABLE IF EXISTS public.students CASCADE;
DROP TABLE IF EXISTS public.lecturers CASCADE;
DROP TABLE IF EXISTS public.departments CASCADE;

-- ==========================================
-- 2. Core Tables
-- ==========================================

-- Departments Table
CREATE TABLE public.departments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name TEXT NOT NULL UNIQUE,
    code TEXT UNIQUE,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- Lecturers Table (Linked to Supabase Auth)
CREATE TABLE public.lecturers (
    id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    full_name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    registration_id TEXT UNIQUE,
    department_id UUID REFERENCES public.departments(id) ON DELETE SET NULL,
    is_admin BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- Students Table
CREATE TABLE public.students (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    student_number TEXT NOT NULL UNIQUE,
    full_name TEXT NOT NULL,
    email TEXT UNIQUE,
    phone TEXT,
    qr_code TEXT UNIQUE,
    department_id UUID REFERENCES public.departments(id) ON DELETE SET NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- Courses Table
CREATE TABLE public.courses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name TEXT NOT NULL,
    code TEXT NOT NULL UNIQUE,
    department_id UUID REFERENCES public.departments(id) ON DELETE CASCADE,
    lecturer_id UUID REFERENCES public.lecturers(id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- Sections (Class Groups) Table
CREATE TABLE public.sections (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    course_id UUID REFERENCES public.courses(id) ON DELETE CASCADE,
    name TEXT NOT NULL,
    semester INTEGER NOT NULL,
    academic_year INTEGER NOT NULL,
    lecturer_id UUID REFERENCES public.lecturers(id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- Enrollments Junction Table
CREATE TABLE public.enrollments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    student_id UUID REFERENCES public.students(id) ON DELETE CASCADE,
    section_id UUID REFERENCES public.sections(id) ON DELETE CASCADE,
    enrolled_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    UNIQUE(student_id, section_id)
);

-- Attendance Logs Table (Real-Time Optimized)
CREATE TABLE public.attendance_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    student_id UUID REFERENCES public.students(id) ON DELETE CASCADE,
    section_id UUID REFERENCES public.sections(id) ON DELETE CASCADE,
    status TEXT NOT NULL CHECK (status IN ('Present', 'Absent', 'Late', 'Excused')),
    session_date TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    is_verified BOOLEAN DEFAULT FALSE,
    is_excused BOOLEAN DEFAULT FALSE,
    verified_by UUID REFERENCES public.lecturers(id),
    notes TEXT,
    location_coords TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- ==========================================
-- 3. Row Level Security (RLS)
-- ==========================================

ALTER TABLE public.departments ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.lecturers ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.students ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.courses ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.sections ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.enrollments ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.attendance_logs ENABLE ROW LEVEL SECURITY;

-- Lecturers Policies
CREATE POLICY "Lecturers can view all departments" ON public.departments FOR SELECT USING (true);
CREATE POLICY "Lecturers can view their own profile" ON public.lecturers FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Lecturers can view students in their sections" ON public.students
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM public.enrollments e
            JOIN public.sections s ON e.section_id = s.id
            WHERE e.student_id = public.students.id AND s.lecturer_id = auth.uid()
        )
        OR (SELECT is_admin FROM public.lecturers WHERE id = auth.uid())
    );

CREATE POLICY "Lecturers can manage sections they teach" ON public.sections
    FOR ALL USING (lecturer_id = auth.uid() OR (SELECT is_admin FROM public.lecturers WHERE id = auth.uid()));

CREATE POLICY "Lecturers can manage attendance for their sections" ON public.attendance_logs
    FOR ALL USING (
        EXISTS (
            SELECT 1 FROM public.sections s
            WHERE s.id = section_id AND s.lecturer_id = auth.uid()
        )
        OR (SELECT is_admin FROM public.lecturers WHERE id = auth.uid())
    );

-- Student Policies
CREATE POLICY "Students can view their own attendance" ON public.attendance_logs
    FOR SELECT USING (
        student_id IN (SELECT id FROM public.students WHERE email = auth.jwt()->>'email')
    );

-- ==========================================
-- 4. Triggers & Functions
-- ==========================================

CREATE OR REPLACE FUNCTION public.handle_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_updated_at_departments BEFORE UPDATE ON public.departments FOR EACH ROW EXECUTE FUNCTION public.handle_updated_at();
CREATE TRIGGER set_updated_at_lecturers BEFORE UPDATE ON public.lecturers FOR EACH ROW EXECUTE FUNCTION public.handle_updated_at();
CREATE TRIGGER set_updated_at_students BEFORE UPDATE ON public.students FOR EACH ROW EXECUTE FUNCTION public.handle_updated_at();
CREATE TRIGGER set_updated_at_courses BEFORE UPDATE ON public.courses FOR EACH ROW EXECUTE FUNCTION public.handle_updated_at();
CREATE TRIGGER set_updated_at_sections BEFORE UPDATE ON public.sections FOR EACH ROW EXECUTE FUNCTION public.handle_updated_at();
CREATE TRIGGER set_updated_at_attendance_logs BEFORE UPDATE ON public.attendance_logs FOR EACH ROW EXECUTE FUNCTION public.handle_updated_at();

-- ==========================================
-- 5. Real-Time Setup
-- ==========================================

ALTER PUBLICATION supabase_realtime ADD TABLE public.attendance_logs;
ALTER PUBLICATION supabase_realtime ADD TABLE public.students;
ALTER PUBLICATION supabase_realtime ADD TABLE public.sections;

-- ==========================================
-- 6. Helper Functions
-- ==========================================

CREATE OR REPLACE FUNCTION public.get_attendance_percentage(p_student_id UUID, p_section_id UUID)
RETURNS DECIMAL AS $$
DECLARE
    v_total_sessions INTEGER;
    v_present_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO v_total_sessions FROM public.attendance_logs 
    WHERE student_id = p_student_id AND section_id = p_section_id;
    
    SELECT COUNT(*) INTO v_present_count FROM public.attendance_logs 
    WHERE student_id = p_student_id AND section_id = p_section_id AND status = 'Present';
    
    IF v_total_sessions = 0 THEN RETURN 0; END IF;
    RETURN (v_present_count::DECIMAL / v_total_sessions::DECIMAL) * 100;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;
