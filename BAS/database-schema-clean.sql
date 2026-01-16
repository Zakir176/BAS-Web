-- BAS Database Schema for Supabase - CLEAN VERSION
-- Drops everything and rebuilds from scratch
-- Run this if you want to completely reset the database

-- Drop all existing objects in reverse order of dependencies
DROP TRIGGER IF EXISTS trigger_update_absence_warnings ON attendance;
DROP TRIGGER IF EXISTS update_students_updated_at ON students;
DROP TRIGGER IF EXISTS update_teachers_updated_at ON teachers;
DROP TRIGGER IF EXISTS update_courses_updated_at ON courses;

DROP POLICY IF EXISTS "Teachers can manage warnings for their courses" ON absence_warnings;
DROP POLICY IF EXISTS "Teachers can manage enrollments for their courses" ON enrollments;
DROP POLICY IF EXISTS "Teachers can view enrollments for their courses" ON enrollments;
DROP POLICY IF EXISTS "Teachers can manage attendance for their sessions" ON attendance;
DROP POLICY IF EXISTS "Teachers can manage their sessions" ON sessions;
DROP POLICY IF EXISTS "Teachers can update their courses" ON courses;
DROP POLICY IF EXISTS "Teachers can view their courses" ON courses;
DROP POLICY IF EXISTS "Teachers can update own profile" ON teachers;
DROP POLICY IF EXISTS "Teachers can view own profile" ON teachers;

DROP VIEW IF EXISTS student_absence_summary;

DROP TABLE IF EXISTS absence_warnings;
DROP TABLE IF EXISTS attendance;
DROP TABLE IF EXISTS enrollments;
DROP TABLE IF EXISTS sessions;
DROP TABLE IF EXISTS courses;
DROP TABLE IF EXISTS teachers;
DROP TABLE IF EXISTS students;

-- Enable UUID extension for teacher IDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Students Table: Core student information from .xlsx upload
CREATE TABLE students (
    student_id VARCHAR(20) PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    class_section VARCHAR(20),
    qr_code_value VARCHAR(100) UNIQUE,  -- Used for barcode/QR scanning
    email VARCHAR(100) UNIQUE,
    phone VARCHAR(20),
    password VARCHAR(255) NOT NULL,  -- Password for authentication
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Teachers Table: For authentication and ownership
CREATE TABLE teachers (
    teacher_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,  -- Password for authentication
    role VARCHAR(20) DEFAULT 'teacher' CHECK (role IN ('teacher', 'admin')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Courses / Subjects Table
CREATE TABLE courses (
    course_id SERIAL PRIMARY KEY,
    course_name VARCHAR(100) NOT NULL,
    teacher_id UUID REFERENCES teachers(teacher_id),
    max_absences_allowed INTEGER DEFAULT 3,  -- School rule: max 3 misses
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Enrollments: Junction table (many-to-many between students and courses)
CREATE TABLE enrollments (
    enrollment_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    enrolled_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(student_id, course_id)
);

-- Sessions: Each actual class/lesson/meeting
CREATE TABLE sessions (
    session_id SERIAL PRIMARY KEY,
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    session_date DATE NOT NULL,
    session_time TIME,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Attendance: Core record of presence/absence
CREATE TABLE attendance (
    attendance_id SERIAL PRIMARY KEY,
    session_id INTEGER NOT NULL REFERENCES sessions(session_id),
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    status VARCHAR(20) DEFAULT 'Absent' CHECK (status IN ('Present', 'Absent', 'Excused', 'Late')),
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    method VARCHAR(20) DEFAULT 'Manual' CHECK (method IN ('QR', 'Barcode', 'Manual', 'Web')),
    excused_reason TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Absence Warnings: Auto-generated for 3-miss rule
CREATE TABLE absence_warnings (
    warning_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    consecutive_absences INTEGER DEFAULT 0,
    total_absences INTEGER DEFAULT 0,
    warning_level INTEGER DEFAULT 0,  -- 1 = warning (2 misses), 2 = critical (3+)
    sent_to_parent BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE (student_id, course_id)  -- One warning row per student per course
);

-- Academic Progression: Track student academic standing
CREATE TABLE academic_progression (
    progression_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    academic_year INTEGER NOT NULL,  -- e.g., 2024, 2025
    academic_level INTEGER NOT NULL,  -- 1 = Year 1, 2 = Year 2, etc.
    failed_courses INTEGER DEFAULT 0,  -- Number of failed courses this year
    can_progress BOOLEAN DEFAULT TRUE,  -- Can proceed to next year
    progression_status VARCHAR(20) DEFAULT 'normal' CHECK (progression_status IN ('normal', 'probation', 'repeat', 'progress')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(student_id, academic_year)
);

-- Course Prerequisites: Define prerequisite relationships
CREATE TABLE course_prerequisites (
    prerequisite_id SERIAL PRIMARY KEY,
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    prerequisite_course_id INTEGER NOT NULL REFERENCES courses(course_id),
    is_mandatory BOOLEAN DEFAULT TRUE,  -- TRUE = must pass, FALSE = recommended
    minimum_grade VARCHAR(2) DEFAULT 'C',  -- Minimum grade required (C, B, A, etc.)
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(course_id, prerequisite_course_id),
    CHECK (course_id != prerequisite_course_id)  -- Can't be prerequisite of itself
);

-- Student Grades: Track academic performance
CREATE TABLE student_grades (
    grade_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    grade VARCHAR(2) NOT NULL CHECK (grade IN ('A+', 'A', 'B+', 'B', 'C+', 'C', 'D', 'F')),
    gpa_points DECIMAL(3,2) NOT NULL,  -- GPA points (4.0, 3.7, 3.3, etc.)
    academic_year INTEGER NOT NULL,
    semester INTEGER NOT NULL CHECK (semester IN (1, 2)),  -- 1 = First semester, 2 = Second semester
    is_repeat BOOLEAN DEFAULT FALSE,  -- True if repeating the course
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(student_id, course_id, academic_year, semester)
);

-- Course Enrollment with Prerequisite Check: Enhanced enrollment tracking
CREATE TABLE course_enrollments (
    enrollment_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    academic_year INTEGER NOT NULL,
    semester INTEGER NOT NULL CHECK (semester IN (1, 2)),
    enrollment_status VARCHAR(20) DEFAULT 'enrolled' CHECK (enrollment_status IN ('enrolled', 'completed', 'dropped', 'failed')),
    prerequisites_met BOOLEAN DEFAULT FALSE,  -- Track if prerequisites were satisfied
    blocked_reason TEXT,  -- Reason if enrollment blocked
    enrolled_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(student_id, course_id, academic_year, semester)
);

-- Create indexes for better performance
CREATE INDEX idx_students_student_id ON students(student_id);
CREATE INDEX idx_students_email ON students(email);
CREATE INDEX idx_teachers_email ON teachers(email);
CREATE INDEX idx_courses_teacher ON courses(teacher_id);
CREATE INDEX idx_sessions_course_date ON sessions(course_id, session_date);
CREATE INDEX idx_attendance_session_student ON attendance(session_id, student_id);
CREATE INDEX idx_attendance_student_date ON attendance(student_id, timestamp);
CREATE INDEX idx_enrollments_student ON enrollments(student_id);
CREATE INDEX idx_enrollments_course ON enrollments(course_id);
CREATE INDEX idx_warnings_student_course ON absence_warnings(student_id, course_id);

-- Academic progression indexes
CREATE INDEX idx_academic_progression_student ON academic_progression(student_id);
CREATE INDEX idx_academic_progression_year ON academic_progression(academic_year);
CREATE INDEX idx_academic_progression_status ON academic_progression(progression_status);

-- Course prerequisites indexes
CREATE INDEX idx_course_prerequisites_course ON course_prerequisites(course_id);
CREATE INDEX idx_course_prerequisites_prereq ON course_prerequisites(prerequisite_course_id);

-- Student grades indexes
CREATE INDEX idx_student_grades_student ON student_grades(student_id);
CREATE INDEX idx_student_grades_course ON student_grades(course_id);
CREATE INDEX idx_student_grades_year_semester ON student_grades(academic_year, semester);

-- Course enrollment indexes
CREATE INDEX idx_course_enrollments_student ON course_enrollments(student_id);
CREATE INDEX idx_course_enrollments_course ON course_enrollments(course_id);
CREATE INDEX idx_course_enrollments_year_semester ON course_enrollments(academic_year, semester);

-- Enable Row Level Security (RLS)
ALTER TABLE students ENABLE ROW LEVEL SECURITY;
ALTER TABLE teachers ENABLE ROW LEVEL SECURITY;
ALTER TABLE courses ENABLE ROW LEVEL SECURITY;
ALTER TABLE sessions ENABLE ROW LEVEL SECURITY;
ALTER TABLE attendance ENABLE ROW LEVEL SECURITY;
ALTER TABLE enrollments ENABLE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings ENABLE ROW LEVEL SECURITY;
ALTER TABLE academic_progression ENABLE ROW LEVEL SECURITY;
ALTER TABLE course_prerequisites ENABLE ROW LEVEL SECURITY;
ALTER TABLE student_grades ENABLE ROW LEVEL SECURITY;
ALTER TABLE course_enrollments ENABLE ROW LEVEL SECURITY;

-- RLS Policies for teachers table (for authentication)
CREATE POLICY "Teachers can view own profile" ON teachers
  FOR SELECT USING (auth.uid() = teacher_id);

CREATE POLICY "Teachers can update own profile" ON teachers
  FOR UPDATE USING (auth.uid() = teacher_id);

-- RLS Policies for courses table
CREATE POLICY "Teachers can view their courses" ON courses
  FOR SELECT USING (auth.uid() = teacher_id);

CREATE POLICY "Teachers can update their courses" ON courses
  FOR UPDATE USING (auth.uid() = teacher_id);

-- RLS Policies for sessions table
CREATE POLICY "Teachers can manage their sessions" ON sessions
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = sessions.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

-- RLS Policies for attendance table
CREATE POLICY "Teachers can manage attendance for their sessions" ON attendance
  FOR ALL USING (
    EXISTS (
      SELECT s.course_id 
      FROM sessions s 
      WHERE s.session_id = attendance.session_id 
      AND EXISTS (
        SELECT 1 FROM courses c 
        WHERE c.course_id = s.course_id 
        AND c.teacher_id = auth.uid()
      )
    )
  );

-- RLS Policies for enrollments table
CREATE POLICY "Teachers can view enrollments for their courses" ON enrollments
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = enrollments.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

CREATE POLICY "Teachers can manage enrollments for their courses" ON enrollments
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = enrollments.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

-- RLS Policies for absence_warnings
CREATE POLICY "Teachers can manage warnings for their courses" ON absence_warnings
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = absence_warnings.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

-- RLS Policies for academic_progression
CREATE POLICY "Teachers can view academic progression for their students" ON academic_progression
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM course_enrollments ce
      JOIN courses c ON c.course_id = ce.course_id
      WHERE ce.student_id = academic_progression.student_id
      AND c.teacher_id = auth.uid()
    )
  );

-- RLS Policies for course_prerequisites
CREATE POLICY "Teachers can manage prerequisites for their courses" ON course_prerequisites
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = course_prerequisites.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

-- RLS Policies for student_grades
CREATE POLICY "Teachers can manage grades for their courses" ON student_grades
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = student_grades.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

-- RLS Policies for course_enrollments
CREATE POLICY "Teachers can manage enrollments for their courses" ON course_enrollments
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = course_enrollments.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

-- Student View for absence summary
CREATE VIEW student_absence_summary AS
SELECT 
    e.student_id,
    e.course_id,
    c.course_name,
    COUNT(CASE WHEN a.status = 'Absent' THEN 1 END) AS total_absences,
    COUNT(CASE WHEN a.status = 'Absent' AND a.timestamp >= CURRENT_DATE - INTERVAL '30 days' THEN 1 END) AS recent_absences
FROM enrollments e
LEFT JOIN attendance a ON a.student_id = e.student_id AND a.session_id IN (
    SELECT session_id FROM sessions WHERE course_id = e.course_id
)
LEFT JOIN courses c ON c.course_id = e.course_id
GROUP BY e.student_id, e.course_id, c.course_name;

-- Function to check CBU academic progression rules
CREATE OR REPLACE FUNCTION check_cbu_progression(student_uuid VARCHAR(20), current_year INTEGER)
RETURNS TABLE(
  can_progress BOOLEAN,
  failed_courses INTEGER,
  progression_status TEXT,
  blocked_courses TEXT[],
  missing_prerequisites TEXT[]
) AS $$
DECLARE
  failed_count INTEGER;
  status_text TEXT;
  blocked_courses_array TEXT[];
  missing_prereqs_array TEXT[];
BEGIN
  -- Count failed courses for the current academic year
  SELECT COUNT(*) INTO failed_count
  FROM student_grades sg
  WHERE sg.student_id = student_uuid
    AND sg.academic_year = current_year
    AND sg.grade IN ('D', 'F');
  
  -- Determine progression status based on CBU rules
  IF failed_count < 3 THEN
    can_progress := TRUE;
    status_text := CASE 
      WHEN failed_count = 0 THEN 'normal'
      WHEN failed_count = 1 THEN 'normal'
      WHEN failed_count = 2 THEN 'probation'
      ELSE 'normal'
    END;
  ELSE
    can_progress := FALSE;
    status_text := 'repeat';
  END IF;
  
  -- Find courses blocked due to missing prerequisites
  SELECT ARRAY_AGG(c.course_name) INTO blocked_courses_array
  FROM courses c
  JOIN course_prerequisites cp ON c.course_id = cp.course_id
  WHERE NOT EXISTS (
    SELECT 1 FROM student_grades sg
    WHERE sg.student_id = student_uuid
      AND sg.course_id = cp.prerequisite_course_id
      AND sg.grade NOT IN ('D', 'F')
  );
  
  -- Find missing prerequisites for enrolled courses
  SELECT ARRAY_AGG(
    c.course_name || ' requires ' || prereq.course_name || 
    CASE WHEN cp.minimum_grade IS NOT NULL THEN ' (min grade: ' || cp.minimum_grade || ')' ELSE '' END
  ) INTO missing_prereqs_array
  FROM course_enrollments ce
  JOIN courses c ON c.course_id = ce.course_id
  JOIN course_prerequisites cp ON c.course_id = cp.course_id
  WHERE ce.student_id = student_uuid
    AND ce.academic_year = current_year
    AND NOT EXISTS (
      SELECT 1 FROM student_grades sg
      WHERE sg.student_id = student_uuid
        AND sg.course_id = cp.prerequisite_course_id
        AND (sg.grade >= cp.minimum_grade OR cp.minimum_grade IS NULL)
    );
  
  failed_courses := failed_count;
  progression_status := status_text;
  blocked_courses := blocked_courses_array;
  missing_prerequisites := missing_prereqs_array;
  
  RETURN NEXT;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Function to update academic progression automatically
CREATE OR REPLACE FUNCTION update_academic_progression()
RETURNS TRIGGER AS $$
BEGIN
  -- Update or create academic progression record
  INSERT INTO academic_progression (
    student_id, 
    academic_year, 
    academic_level, 
    failed_courses, 
    can_progress, 
    progression_status
  )
  SELECT 
    NEW.student_id,
    NEW.academic_year,
    COALESCE(ap.academic_level, 1) + CASE 
      WHEN (SELECT COUNT(*) FROM student_grades sg 
            WHERE sg.student_id = NEW.student_id 
            AND sg.academic_year = NEW.academic_year 
            AND sg.grade IN ('D', 'F')) < 3 THEN 1 
      ELSE 0 
    END,
    (SELECT COUNT(*) FROM student_grades sg 
     WHERE sg.student_id = NEW.student_id 
     AND sg.academic_year = NEW.academic_year 
     AND sg.grade IN ('D', 'F')),
    (SELECT COUNT(*) FROM student_grades sg 
     WHERE sg.student_id = NEW.student_id 
     AND sg.academic_year = NEW.academic_year 
     AND sg.grade IN ('D', 'F')) < 3,
    CASE 
      WHEN (SELECT COUNT(*) FROM student_grades sg 
            WHERE sg.student_id = NEW.student_id 
            AND sg.academic_year = NEW.academic_year 
            AND sg.grade IN ('D', 'F')) >= 3 THEN 'repeat'
      WHEN (SELECT COUNT(*) FROM student_grades sg 
            WHERE sg.student_id = NEW.student_id 
            AND sg.academic_year = NEW.academic_year 
            AND sg.grade IN ('D', 'F')) = 2 THEN 'probation'
      ELSE 'normal'
    END
  FROM academic_progression ap
  WHERE ap.student_id = NEW.student_id
    AND ap.academic_year = NEW.academic_year - 1
  ON CONFLICT (student_id, academic_year) 
  DO UPDATE SET 
    failed_courses = EXCLUDED.failed_courses,
    can_progress = EXCLUDED.can_progress,
    progression_status = EXCLUDED.progression_status,
    updated_at = NOW();
  
  RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Trigger to update academic progression when grades are recorded
CREATE TRIGGER trigger_update_academic_progression
    AFTER INSERT OR UPDATE ON student_grades
    FOR EACH ROW EXECUTE FUNCTION update_academic_progression();
CREATE OR REPLACE FUNCTION update_absence_warnings()
RETURNS TRIGGER AS $$
BEGIN
    -- Get current absence count for this student in this course
    INSERT INTO absence_warnings (student_id, course_id, total_absences, consecutive_absences, warning_level)
    VALUES (
        NEW.student_id,
        (SELECT s.course_id FROM sessions s WHERE s.session_id = NEW.session_id),
        (SELECT COUNT(*) FROM attendance a 
         WHERE a.student_id = NEW.student_id 
         AND a.session_id IN (
             SELECT s2.session_id FROM sessions s2 
             JOIN courses c ON c.course_id = s2.course_id
             WHERE c.teacher_id = auth.uid()
         )
         AND a.status = 'Absent'),
        (SELECT COUNT(*) FROM attendance a 
         WHERE a.student_id = NEW.student_id 
         AND a.session_id IN (
             SELECT s2.session_id FROM sessions s2 
             JOIN courses c ON c.course_id = s2.course_id
             WHERE c.teacher_id = auth.uid()
         )
         AND a.status = 'Absent' 
         AND a.timestamp >= CURRENT_DATE - INTERVAL '7 days'),
        CASE 
            WHEN (SELECT COUNT(*) FROM attendance a 
                 WHERE a.student_id = NEW.student_id 
                 AND a.session_id IN (
                     SELECT s2.session_id FROM sessions s2 
                     JOIN courses c ON c.course_id = s2.course_id
                     WHERE c.teacher_id = auth.uid()
                 )
                 AND a.status = 'Absent') >= 3 THEN 2
            WHEN (SELECT COUNT(*) FROM attendance a 
                 WHERE a.student_id = NEW.student_id 
                 AND a.session_id IN (
                     SELECT s2.session_id FROM sessions s2 
                     JOIN courses c ON c.course_id = s2.course_id
                     WHERE c.teacher_id = auth.uid()
                 )
                 AND a.status = 'Absent') >= 2 THEN 1
            ELSE 0
        END
    )
    ON CONFLICT (student_id, course_id) 
    DO UPDATE SET 
        total_absences = EXCLUDED.total_absences,
        consecutive_absences = EXCLUDED.consecutive_absences,
        warning_level = EXCLUDED.warning_level;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Trigger to update warnings when attendance is recorded
CREATE TRIGGER trigger_update_absence_warnings
    AFTER INSERT OR UPDATE ON attendance
    FOR EACH ROW EXECUTE FUNCTION update_absence_warnings();

-- Update updated_at timestamp function
CREATE OR REPLACE FUNCTION public.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Create triggers for updated_at
CREATE TRIGGER update_students_updated_at BEFORE UPDATE ON students
  FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER update_teachers_updated_at BEFORE UPDATE ON teachers
  FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();

CREATE TRIGGER update_courses_updated_at BEFORE UPDATE ON courses
  FOR EACH ROW EXECUTE FUNCTION public.update_updated_at_column();
