-- BAS Database Schema for Supabase Auth
-- This schema works with Supabase Auth (no password columns needed)

-- Enable UUID extension for teacher IDs
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Students Table: Core student information (no password - handled by Supabase Auth)
CREATE TABLE IF NOT EXISTS students (
    student_id VARCHAR(20) PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    class_section VARCHAR(20),
    qr_code_value VARCHAR(100) UNIQUE,  -- Used for barcode/QR scanning
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Teachers Table: For authentication and ownership (no password - handled by Supabase Auth)
CREATE TABLE IF NOT EXISTS teachers (
    teacher_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    role VARCHAR(20) DEFAULT 'teacher' CHECK (role IN ('teacher', 'admin')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Courses / Subjects Table
CREATE TABLE IF NOT EXISTS courses (
    course_id SERIAL PRIMARY KEY,
    course_name VARCHAR(100) NOT NULL,
    teacher_id UUID REFERENCES teachers(teacher_id),
    max_absences_allowed INTEGER DEFAULT 3,  -- School rule: max 3 misses
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Enrollments: Junction table (many-to-many between students and courses)
CREATE TABLE IF NOT EXISTS enrollments (
    enrollment_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    enrolled_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(student_id, course_id)
);

-- Sessions: Each actual class/lesson/meeting
CREATE TABLE IF NOT EXISTS sessions (
    session_id SERIAL PRIMARY KEY,
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    session_date DATE NOT NULL,
    session_time TIME,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Attendance: Individual attendance records
CREATE TABLE IF NOT EXISTS attendance (
    attendance_id SERIAL PRIMARY KEY,
    session_id INTEGER NOT NULL REFERENCES sessions(session_id),
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    status VARCHAR(20) DEFAULT 'Absent' CHECK (status IN ('Present', 'Absent', 'Excused', 'Late')),
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    method VARCHAR(20) DEFAULT 'Manual' CHECK (method IN ('QR', 'Barcode', 'Manual', 'Web')),
    excused_reason TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Absence Warnings: Track warning levels for students
CREATE TABLE IF NOT EXISTS absence_warnings (
    warning_id SERIAL PRIMARY KEY,
    student_id VARCHAR(20) NOT NULL REFERENCES students(student_id),
    course_id INTEGER NOT NULL REFERENCES courses(course_id),
    consecutive_absences INTEGER DEFAULT 0,
    total_absences INTEGER DEFAULT 0,
    warning_level INTEGER DEFAULT 0,
    sent_to_parent BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE (student_id, course_id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_students_student_id ON students(student_id);
CREATE INDEX IF NOT EXISTS idx_students_email ON students(email);
CREATE INDEX IF NOT EXISTS idx_teachers_email ON teachers(email);
CREATE INDEX IF NOT EXISTS idx_courses_teacher ON courses(teacher_id);
CREATE INDEX IF NOT EXISTS idx_sessions_course_date ON sessions(course_id, session_date);
CREATE INDEX IF NOT EXISTS idx_attendance_session_student ON attendance(session_id, student_id);
CREATE INDEX IF NOT EXISTS idx_attendance_student_date ON attendance(student_id, timestamp);
CREATE INDEX IF NOT EXISTS idx_enrollments_student ON enrollments(student_id);
CREATE INDEX IF NOT EXISTS idx_enrollments_course ON enrollments(course_id);
CREATE INDEX IF NOT EXISTS idx_warnings_student_course ON absence_warnings(student_id, course_id);

-- Enable Row Level Security (RLS)
ALTER TABLE students ENABLE ROW LEVEL SECURITY;
ALTER TABLE teachers ENABLE ROW LEVEL SECURITY;
ALTER TABLE courses ENABLE ROW LEVEL SECURITY;
ALTER TABLE sessions ENABLE ROW LEVEL SECURITY;
ALTER TABLE attendance ENABLE ROW LEVEL SECURITY;
ALTER TABLE enrollments ENABLE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings ENABLE ROW LEVEL SECURITY;

-- RLS Policies for students table
CREATE POLICY "Students can view own profile" ON students
  FOR SELECT USING (auth.uid()::text = student_id);

CREATE POLICY "Students can update own profile" ON students
  FOR UPDATE USING (auth.uid()::text = student_id);

-- RLS Policies for teachers table
CREATE POLICY "Teachers can view own profile" ON teachers
  FOR SELECT USING (auth.uid() = teacher_id);

CREATE POLICY "Teachers can update own profile" ON teachers
  FOR UPDATE USING (auth.uid() = teacher_id);

-- RLS Policies for courses table
CREATE POLICY "Teachers can view their courses" ON courses
  FOR ALL USING (auth.uid() = teacher_id);

CREATE POLICY "Students can view enrolled courses" ON courses
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM enrollments 
      WHERE enrollments.course_id = courses.course_id 
      AND enrollments.student_id = auth.uid()::text
    )
  );

-- RLS Policies for sessions table
CREATE POLICY "Teachers can manage their sessions" ON sessions
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = sessions.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

CREATE POLICY "Students can view sessions for enrolled courses" ON sessions
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM enrollments e
      JOIN courses c ON c.course_id = e.course_id
      WHERE e.course_id = sessions.course_id 
      AND e.student_id = auth.uid()::text
    )
  );

-- RLS Policies for attendance table
CREATE POLICY "Teachers can manage attendance for their sessions" ON attendance
  FOR ALL USING (
    EXISTS (
      SELECT 1 FROM sessions s
      JOIN courses c ON c.course_id = s.course_id
      WHERE s.session_id = attendance.session_id 
      AND c.teacher_id = auth.uid()
    )
  );

CREATE POLICY "Students can view own attendance" ON attendance
  FOR SELECT USING (auth.uid()::text = student_id);

-- RLS Policies for enrollments table
CREATE POLICY "Teachers can view enrollments for their courses" ON enrollments
  FOR SELECT USING (
    EXISTS (
      SELECT 1 FROM courses 
      WHERE courses.course_id = enrollments.course_id 
      AND courses.teacher_id = auth.uid()
    )
  );

CREATE POLICY "Students can view own enrollments" ON enrollments
  FOR SELECT USING (auth.uid()::text = student_id);

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

CREATE POLICY "Students can view own warnings" ON absence_warnings
  FOR SELECT USING (auth.uid()::text = student_id);

-- Function to update absence warnings automatically
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
             SELECT s.session_id FROM sessions s 
             WHERE s.course_id = (SELECT course_id FROM sessions WHERE session_id = NEW.session_id)
         )
         AND a.status = 'Absent'),
        (SELECT COUNT(*) FROM attendance a 
         WHERE a.student_id = NEW.student_id 
         AND a.session_id IN (
             SELECT s.session_id FROM sessions s 
             WHERE s.course_id = (SELECT course_id FROM sessions WHERE session_id = NEW.session_id)
         )
         AND a.status = 'Absent'
         AND a.timestamp >= CURRENT_DATE - INTERVAL '7 days'),
        CASE 
            WHEN (SELECT COUNT(*) FROM attendance a 
                 WHERE a.student_id = NEW.student_id 
                 AND a.session_id IN (
                     SELECT s.session_id FROM sessions s 
                     WHERE s.course_id = (SELECT course_id FROM sessions WHERE session_id = NEW.session_id)
                 )
                 AND a.status = 'Absent') >= 3 THEN 2
            WHEN (SELECT COUNT(*) FROM attendance a 
                 WHERE a.student_id = NEW.student_id 
                 AND a.session_id IN (
                     SELECT s.session_id FROM sessions s 
                     WHERE s.course_id = (SELECT course_id FROM sessions WHERE session_id = NEW.session_id)
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

-- Trigger to update absence warnings when attendance is recorded
CREATE TRIGGER trigger_update_absence_warnings
    AFTER INSERT OR UPDATE ON attendance
    FOR EACH ROW EXECUTE FUNCTION update_absence_warnings();

-- Function to automatically update updated_at columns
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Triggers for updated_at columns
CREATE TRIGGER update_students_updated_at BEFORE UPDATE ON students
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_teachers_updated_at BEFORE UPDATE ON teachers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_courses_updated_at BEFORE UPDATE ON courses
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
