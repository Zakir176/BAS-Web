# Signup Debugging Guide

## 🔍 Debug Steps

### **Step 1: Run Debug SQL Script**
1. Copy `DEBUG_SIGNUP.sql` content
2. Paste in Supabase SQL Editor
3. Execute the script
4. **Look for any errors** - this will tell us if tables/columns exist

### **Step 2: Check Browser Console**
1. Open browser dev tools (F12)
2. Go to Console tab
3. Try signing up (student or lecturer)
4. **Look for specific error messages** in console

### **Step 3: Check Network Tab**
1. In dev tools, go to Network tab
2. Try signing up again
3. Look for failed requests to Supabase
4. Click on the failed request to see details

### **Step 4: Test Manual Insert**
The debug script includes manual INSERT tests. If these fail, the issue is with:
- Table permissions
- Missing columns
- Database constraints

### **Step 5: Check Environment Variables**
Make sure your `.env` file has:
```env
VITE_SUPABASE_URL=https://oxvdolxolwcpafhuloin.supabase.co
VITE_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 🚨 Common Issues & Solutions

### **Issue 1: "column 'password' does not exist"**
**Solution**: Run the updated database schema with password columns

### **Issue 2: "permission denied for schema public"**
**Solution**: Run `COMPLETE_PERMISSION_FIX.sql`

### **Issue 3: "relation 'students' does not exist"**
**Solution**: Run the database schema to create tables

### **Issue 4: "duplicate key value violates unique constraint"**
**Solution**: Use different email/student_id for testing

## 📋 What to Report Back

Please tell me:
1. **What errors** you see in the SQL debug script
2. **What errors** you see in browser console
3. **What happens** in the Network tab
4. **Which specific step** fails (student vs lecturer signup)

## 🔧 Quick Test

Try this simple test in browser console:
```javascript
// Test Supabase connection
import { supabase } from './src/supabase.js';

// Test if we can read from students table
supabase.from('students').select('*').limit(1)
  .then(data => console.log('Read test:', data))
  .catch(error => console.error('Read error:', error));
```

**Run the debug script and let me know what specific errors you see!** 🔍
