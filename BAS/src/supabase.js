// src/supabase.js
import { createClient } from '@supabase/supabase-js'

// IMPORTANT: Replace with your actual Supabase project URL and Anon Key
// You should store these in environment variables
const supabaseUrl = 'https://oxvdolxolwcpafhuloin.supabase.co'
const supabaseAnonKey = 'sb_publishable_VsorCbzl0jdd_sG1bcG2yw_5LS3wymC'

export const supabase = createClient(supabaseUrl, supabaseAnonKey)
