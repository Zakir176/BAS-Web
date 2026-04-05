import { supabase } from '@/core/api/supabase'

export const attendanceService = {
  /**
   * Marks a student as present for a specific section/session
   */
  async markPresent(studentId, sectionId) {
    const { data, error } = await supabase
      .from('attendance_logs')
      .insert({
        student_id: studentId,
        section_id: sectionId,
        status: 'Present',
        session_date: new Date().toISOString()
      })
      .select()
      .single()

    if (error) {
      if (error.code === '23505') return { success: true, alreadyMarked: true }
      throw error
    }
    return { success: true, data }
  },

  /**
   * Identifies a student by their barcode (student_number) and marks them present
   */
  async markByBarcode(barcode, sectionId) {
    const studentId = barcode.trim()
    
    const { data: student, error: fetchError } = await supabase
      .from('students')
      .select('id, full_name')
      .eq('student_number', studentId)
      .maybeSingle()

    if (fetchError) throw fetchError
    if (!student) return { success: false, error: 'Student not found' }

    const result = await this.markPresent(student.id, sectionId)
    return { ...result, studentName: student.full_name, studentUUID: student.id }
  },

  /**
   * Fetches recent attendance activity for a lecturer's sections
   */
  async getRecentActivity(sectionIds, limit = 8) {
    const { data, error } = await supabase
      .from('attendance_logs')
      .select('*, students(full_name), sections(name, courses(name))')
      .in('section_id', sectionIds)
      .order('session_date', { ascending: false })
      .limit(limit)

    if (error) throw error
    return data.map(l => ({
      id: l.id,
      course_name: l.sections?.courses?.name || l.sections?.name,
      student_name: l.students?.full_name,
      session_date: l.session_date
    }))
  }
}
