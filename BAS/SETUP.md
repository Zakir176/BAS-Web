# BAS - Setup Instructions

## 🚀 Quick Start

### 1. Database Setup
1. Go to your Supabase project: https://supabase.com/dashboard
2. Navigate to **SQL Editor**
3. Copy and paste the contents of `database-schema.sql`
4. Run the SQL script to create all tables and policies

### 2. Authentication Ready
✅ Supabase authentication is now configured
✅ Student signup/login will work with real database
✅ Session management is active

### 3. Test the Application
1. Start the development server: `npm run dev`
2. Navigate to `http://localhost:5173`
3. Test student signup: `/student-signup`
4. Test student login: `/student-login`

## 📋 Current Status

### ✅ Working Features
- **Real Authentication**: Supabase auth integration
- **Student Registration**: Creates user + profile in database
- **Student Login**: Validates credentials and creates session
- **Session Management**: Automatic login state tracking
- **Logout**: Proper session termination
- **UI Components**: All pages with proper styling

### 🔄 Next Steps (Priority Order)

#### 1. Teacher Authentication
- Add teacher signup/login pages
- Update auth composable to handle teacher roles
- Add role-based routing

#### 2. Database Integration
- Replace mock data in StudentHomepage with real queries
- Add attendance tracking functionality
- Implement course management

#### 3. File Upload Integration
- Connect StudentUploadPage to Supabase Storage
- Add file validation and progress tracking
- Implement file management

#### 4. Barcode Scanner
- Add QR code scanning functionality
- Connect to attendance marking
- Mobile camera integration

## 🔧 Technical Details

### Authentication Flow
1. Student signs up → Creates auth user + student profile
2. Student logs in → Validates credentials → Creates session
3. Session persists across page refreshes
4. Navbar updates based on auth state

### Database Tables Created
- `students` - Student profiles (student_id as primary key)
- `teachers` - Teacher profiles  
- `courses` - Course information
- `sessions` - Class sessions/meetings
- `attendance` - Individual attendance records
- `enrollments` - Course enrollments
- `absence_warnings` - Automatic absence tracking

### Key Schema Changes
- **Students**: Use `student_id` (VARCHAR) as primary key instead of UUID
- **Teachers**: Use `teacher_id` (UUID) for authentication
- **Courses**: Use `course_id` (SERIAL) for simple integer IDs
- **Attendance**: Tracks status (Present/Absent/Excused/Late) and method (QR/Barcode/Manual/Web)
- **Auto-warnings**: Triggers automatically create warnings after 2+ absences

### Security Features
- Row Level Security (RLS) enabled
- Teachers can only access their courses and enrolled students
- Students can only view their own attendance records
- Automatic absence warning system (2 misses = warning, 3+ = critical)

## 🎯 What to Test First

1. **Student Registration**
   - Fill out the signup form
   - Check if the user is created in Supabase Auth
   - Verify the student profile is created in the database
   - Confirm `student_id` is used as primary key

2. **Student Login**
   - Use student ID + password
   - Verify session is created
   - Check navbar shows authenticated state

3. **Logout**
   - Click logout button
   - Verify session is cleared
   - Check navbar updates to logged out state

## 🐛 Common Issues

### "User already registered" error
- Email is already in use
- Check Auth users table in Supabase

### "Invalid login credentials" error
- Wrong password or email format
- For student login, use: `student_id@student.bas.edu`
- Example: `2024001@student.bas.edu`

### "column does not exist" error
- This is likely due to RLS policies trying to reference non-existent columns
- The updated schema temporarily disables RLS for students table
- Re-run the SQL schema to fix this issue

### Database errors
- Make sure SQL schema was executed completely
- Check table permissions in Supabase
- Verify RLS policies are applied correctly
- **Students table RLS is temporarily disabled** for testing

### "duplicate key" errors
- `students_student_id_key`: Student ID already exists
- `students_email_key`: Email already exists

## 📞 Need Help?

1. Check browser console for errors
2. Verify Supabase project settings
3. Ensure SQL schema was executed completely
4. Check network tab for API calls
5. Review the new schema structure (student_id as primary key)
