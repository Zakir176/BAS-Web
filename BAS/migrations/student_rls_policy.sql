-- Fix for Student Profile Access
-- This file enables Row Level Security for the students table and adds policies
-- to allow students to view and update their own profiles.

-- 1. Enable Row Level Security for the students table
ALTER TABLE public.students ENABLE ROW LEVEL SECURITY;

-- 2. Create a policy for SELECT access
-- This policy allows a user to select a record from the students table
-- if the email in the table matches the email of the authenticated user.
CREATE POLICY "Students can view their own profile"
ON public.students
FOR SELECT
USING (auth.email() = email);

-- 3. Create a policy for UPDATE access
-- This policy allows a user to update their own record in the students table.
CREATE POLICY "Students can update their own profile"
ON public.students
FOR UPDATE
USING (auth.email() = email);
