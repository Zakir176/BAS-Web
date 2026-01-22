-- COMPLETE RLS POLICY FOR STUDENTS TABLE
-- This script provides all necessary Row Level Security policies for the public.students table.
-- It allows students to sign up, view their own profile, and update it.

-- 1. Ensure RLS is enabled on the students table.
ALTER TABLE public.students ENABLE ROW LEVEL SECURITY;

-- 2. Drop old/debugging policies if they exist to ensure a clean slate.
DROP POLICY IF EXISTS "Students can view their own profile" ON public.students;
DROP POLICY IF EXISTS "Students can update their own profile" ON public.students;
DROP POLICY IF EXISTS "DEBUG - Allow any authenticated user to select students" ON public.students;
DROP POLICY IF EXISTS "Students can create their own profile" ON public.students;

-- 3. Policy for SELECT: Allow students to view their own profile.
-- The user's JWT must contain an email that matches the email in the row they want to see.
CREATE POLICY "Students can view their own profile"
ON public.students
FOR SELECT
USING (auth.email() = email);

-- 4. Policy for UPDATE: Allow students to update their own profile.
-- The user can only update a row where the email matches their own.
CREATE POLICY "Students can update their own profile"
ON public.students
FOR UPDATE
USING (auth.email() = email);

-- 5. Policy for INSERT: Allow a new user to create their own student profile.
-- The new row's email must match the email of the user creating it.
CREATE POLICY "Students can create their own profile"
ON public.students
FOR INSERT
WITH CHECK (auth.email() = email);
