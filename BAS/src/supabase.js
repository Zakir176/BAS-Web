// src/supabase.js
import { createClient } from "@supabase/supabase-js";

// Uses environment variables from .env or .env.local
const supabaseUrl = import.meta.env.VITE_SUPABASE_URL;
const supabaseAnonKey = import.meta.env.VITE_SUPABASE_ANON_KEY;

if (!supabaseUrl || !supabaseAnonKey) {
  console.error("Supabase environment variables are missing! Check your .env file.");
}

export const supabase = createClient(supabaseUrl, supabaseAnonKey);
