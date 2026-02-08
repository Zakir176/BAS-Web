// src/supabase.js
import { createClient } from "@supabase/supabase-js";

// IMPORTANT: Replace with your actual Supabase project URL and Anon Key
// You should store these in environment variables
const supabaseUrl = "https://oxvdolxolwcpafhuloin.supabase.co";
const supabaseAnonKey =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im94dmRvbHhvbHdjcGFmaHVsb2luIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjgxNDg4ODcsImV4cCI6MjA4MzcyNDg4N30.DCGiYMPHWrVXfipfeAmFIIRAhazOzgKSuaH653DRKRU";

export const supabase = createClient(supabaseUrl, supabaseAnonKey);
