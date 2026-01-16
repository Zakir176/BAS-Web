-- Complete Permission Fix for BAS System
-- Run this in Supabase SQL Editor to fix all permission issues

-- Step 1: Drop all existing RLS policies
DROP POLICY IF EXISTS "Teachers can view own profile" ON teachers;
DROP POLICY IF EXISTS "Teachers can update own profile" ON teachers;
DROP POLICY IF EXISTS "Teachers can view their courses" ON courses;
DROP POLICY IF EXISTS "Teachers can update their courses" ON courses;
DROP POLICY IF EXISTS "Teachers can manage their sessions" ON sessions;
DROP POLICY IF EXISTS "Teachers can manage attendance for their sessions" ON attendance;
DROP POLICY IF EXISTS "Teachers can view enrollments for their courses" ON enrollments;
DROP POLICY IF EXISTS "Teachers can manage enrollments for their courses" ON enrollments;
DROP POLICY IF EXISTS "Teachers can manage warnings for their courses" ON absence_warnings;

-- Step 2: Disable RLS on all tables
ALTER TABLE students DISABLE ROW LEVEL SECURITY;
ALTER TABLE teachers DISABLE ROW LEVEL SECURITY;
ALTER TABLE courses DISABLE ROW LEVEL SECURITY;
ALTER TABLE sessions DISABLE ROW LEVEL SECURITY;
ALTER TABLE attendance DISABLE ROW LEVEL SECURITY;
ALTER TABLE enrollments DISABLE ROW LEVEL SECURITY;
ALTER TABLE absence_warnings DISABLE ROW LEVEL SECURITY;

-- Step 3: Grant comprehensive permissions to authenticated users
GRANT ALL ON ALL TABLES IN SCHEMA public TO authenticated;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO authenticated;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO authenticated;

-- Step 4: Grant read permissions to anonymous users (for public access)
GRANT SELECT ON ALL TABLES IN SCHEMA public TO anon;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO anon;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO anon;

-- Step 5: Reset Supabase Auth permissions (if needed)
-- This ensures the service role has proper access
GRANT ALL ON ALL TABLES IN SCHEMA public TO service_role;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO service_role;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO service_role;

-- Step 6: Verify permissions (optional - you can run this to check)
SELECT 
    table_name,
    privilege_type,
    grantee
FROM information_schema.role_table_grants 
WHERE table_schema = 'public' 
AND grantee IN ('authenticated', 'anon', 'service_role')
ORDER BY table_name, privilege_type;
