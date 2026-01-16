-- FINAL PERMISSION FIX - Run this in Supabase SQL Editor
-- This completely removes all RLS and grants proper permissions

-- Step 1: Remove ALL RLS policies completely
DROP POLICY IF EXISTS "Teachers can view own profile" ON teachers;
DROP POLICY IF EXISTS "Teachers can update own profile" ON teachers;
DROP POLICY IF EXISTS "Teachers can view their courses" ON courses;
DROP POLICY IF EXISTS "Teachers can update their courses" ON courses;
DROP POLICY IF EXISTS "Teachers can manage their sessions" ON sessions;
DROP POLICY IF EXISTS "Teachers can manage attendance for their sessions" ON attendance;
DROP POLICY IF EXISTS "Teachers can view enrollments for their courses" ON enrollments;
DROP POLICY IF EXISTS "Teachers can manage enrollments for their courses" ON enrollments;
DROP POLICY IF EXISTS "Teachers can manage warnings for their courses" ON absence_warnings;

-- Step 2: DISABLE RLS on ALL tables
ALTER TABLE students DISABLE ROW LEVEL SECURITY;
ALTER TABLE teachers DISABLE ROW LEVEL SECURITY;
ALTER TABLE courses DISABLE ROW LEVEL SECURITY;
ALTER TABLE sessions DISABLE ROW LEVEL SECURITY;
ALTER TABLE attendance DISABLE ROW LEVEL SECURITY;
ALTER TABLE enrollments DISABLE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings DISABLE ROW LEVEL SECURITY;

-- Step 3: Grant ALL permissions to authenticated users
GRANT ALL ON ALL TABLES IN SCHEMA public TO authenticated;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO authenticated;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO authenticated;

-- Step 4: Grant ALL permissions to service role (Supabase internal)
GRANT ALL ON ALL TABLES IN SCHEMA public TO service_role;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO service_role;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO service_role;

-- Step 5: Grant SELECT permissions to anonymous users
GRANT SELECT ON ALL TABLES IN SCHEMA public TO anon;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO anon;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO anon;

-- Step 6: Grant ALL to postgres superuser
GRANT ALL ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO postgres;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO postgres;

-- Step 7: Remove any remaining RLS (double-check)
ALTER TABLE students FORCE ROW LEVEL SECURITY;
ALTER TABLE students DISABLE ROW LEVEL SECURITY;
ALTER TABLE teachers FORCE ROW LEVEL SECURITY;
ALTER TABLE teachers DISABLE ROW LEVEL SECURITY;
ALTER TABLE courses FORCE ROW LEVEL SECURITY;
ALTER TABLE courses DISABLE ROW LEVEL SECURITY;
ALTER TABLE sessions FORCE ROW LEVEL SECURITY;
ALTER TABLE sessions DISABLE ROW LEVEL SECURITY;
ALTER TABLE attendance FORCE ROW LEVEL SECURITY;
ALTER TABLE attendance DISABLE ROW LEVEL SECURITY;
ALTER TABLE enrollments FORCE ROW LEVEL SECURITY;
ALTER TABLE enrollments DISABLE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings FORCE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings DISABLE ROW LEVEL SECURITY;

-- Step 8: Final verification - Check permissions
SELECT 
    table_name,
    privilege_type,
    grantee
FROM information_schema.role_table_grants 
WHERE table_schema = 'public' 
    AND table_name IN ('students', 'teachers', 'courses', 'sessions', 'attendance', 'enrollments', 'absence_warnings')
ORDER BY table_name, privilege_type;
