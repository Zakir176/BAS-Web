-- DEBUGGING SCRIPT for Student Profile Access
-- This file is for debugging purposes only. It will temporarily grant broader access
-- to the students table to diagnose the "permission denied" error.

-- 1. Drop the existing policies on the students table.
-- It's safe to run this even if the policies don't exist.
DROP POLICY IF EXISTS "Students can view their own profile" ON public.students;
DROP POLICY IF EXISTS "Students can update their own profile" ON public.students;

-- 2. Create a temporary, permissive SELECT policy.
-- This policy allows ANY authenticated user to read ANY record from the students table.
-- If this query succeeds where the previous one failed, it proves that the issue is
-- not with RLS itself, but with the data: there is likely no 'students' record
-- that matches the email of the logged-in user.
CREATE POLICY "DEBUG - Allow any authenticated user to select students"
ON public.students
FOR SELECT
USING (auth.role() = 'authenticated');

-- NOTE: We are not creating an UPDATE policy to minimize security risks during debugging.
