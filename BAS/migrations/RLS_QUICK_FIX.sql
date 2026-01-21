-- Quick RLS Fix for Permission Denied Error
-- Run this in Supabase SQL Editor to disable RLS temporarily

-- Disable RLS on all tables to fix permission issues
ALTER TABLE students DISABLE ROW LEVEL SECURITY;
ALTER TABLE teachers DISABLE ROW LEVEL SECURITY;
ALTER TABLE courses DISABLE ROW LEVEL SECURITY;
ALTER TABLE sessions DISABLE ROW LEVEL SECURITY;
ALTER TABLE attendance DISABLE ROW LEVEL SECURITY;
ALTER TABLE enrollments DISABLE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings DISABLE ROW LEVEL SECURITY;

-- If you have the new academic tables, also disable RLS on them:
-- ALTER TABLE academic_progression DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE course_prerequisites DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE student_grades DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE course_enrollments DISABLE ROW LEVEL SECURITY;

-- Grant public access to all tables (temporary fix)
GRANT ALL ON ALL TABLES IN SCHEMA public TO anon;
GRANT ALL ON ALL TABLES IN SCHEMA public TO authenticated;

-- Grant usage on sequences
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO anon;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO authenticated;

-- Grant execute on functions
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO anon;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO authenticated;
