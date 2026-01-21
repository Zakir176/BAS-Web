-- Debug Signup Issues - Run this in Supabase SQL Editor

-- Step 1: Check if tables exist and have password columns
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns 
WHERE table_name IN ('students', 'teachers') 
    AND table_schema = 'public'
ORDER BY table_name, ordinal_position;

-- Step 2: Check current permissions on tables
SELECT 
    table_name,
    privilege_type,
    grantee
FROM information_schema.role_table_grants 
WHERE table_schema = 'public' 
    AND table_name IN ('students', 'teachers')
    AND grantee IN ('authenticated', 'anon', 'service_role')
ORDER BY table_name, privilege_type;

-- Step 3: Test manual insert (try this to see if it works)
-- Try inserting a test student (replace with test data)
INSERT INTO students (
    student_id, 
    full_name, 
    email, 
    password, 
    class_section, 
    qr_code_value
) VALUES (
    'TEST001', 
    'Test Student', 
    'test@example.com', 
    'password123', 
    'CS101', 
    'TEST001'
);

-- Step 4: Check if the insert worked
SELECT * FROM students WHERE student_id = 'TEST001';

-- Step 5: Clean up test data
DELETE FROM students WHERE student_id = 'TEST001';

-- Step 6: Try inserting a test teacher
INSERT INTO teachers (
    full_name, 
    email, 
    password, 
    role
) VALUES (
    'Test Teacher', 
    'teacher@example.com', 
    'password123', 
    'teacher'
);

-- Step 7: Check if teacher insert worked
SELECT * FROM teachers WHERE email = 'teacher@example.com';

-- Step 8: Clean up test data
DELETE FROM teachers WHERE email = 'teacher@example.com';
