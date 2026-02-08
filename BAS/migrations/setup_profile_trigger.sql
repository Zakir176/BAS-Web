-- Creates a function that will be triggered on new user creation.
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
DECLARE
  user_role TEXT;
BEGIN
  -- Extract the role from the user's metadata
  user_role := new.raw_user_meta_data->>'role';

  -- Insert into the appropriate table based on the role
  IF user_role = 'student' THEN
    INSERT INTO public.students (id, email, full_name, student_id, class_section, qr_code_value)
    VALUES (
      new.id,
      new.email,
      new.raw_user_meta_data->>'full_name',
      new.raw_user_meta_data->>'student_id',
      new.raw_user_meta_data->>'class_section',
      new.raw_user_meta_data->>'student_id' -- Using student_id for QR code
    );
  ELSIF user_role = 'lecturer' THEN
    INSERT INTO public.teachers (id, email, full_name, role)
    VALUES (
      new.id,
      new.email,
      new.raw_user_meta_data->>'full_name',
      user_role
    );
  END IF;
  
  RETURN new;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Creates the trigger that fires the function after a new user is created.
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
CREATE TRIGGER on_auth_user_created
  AFTER INSERT ON auth.users
  FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();
