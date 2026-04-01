import { supabase } from '@/core/api/supabase'

export const courseService = {
  /**
   * Fetches the lecturer's profile and their assigned courses/sections
   */
  async getLecturerDashboardData(lecturerId) {
    // 1. Fetch Profile
    const { data: lecturer } = await supabase
      .from('lecturers')
      .select('full_name')
      .eq('id', lecturerId)
      .maybeSingle()

    // 2. Fetch Sections with Enrollment Counts
    const { data: sections, error } = await supabase
      .from('sections')
      .select('id, name, course_id, courses(name), enrollments(count)')
      .eq('lecturer_id', lecturerId)

    if (error) throw error

    // 3. Fetch Attendance Stats for each section
    const sectionsWithStats = await Promise.all(sections.map(async (s) => {
      const { count: attendanceCount } = await supabase
        .from('attendance_logs')
        .select('*', { count: 'exact', head: true })
        .eq('section_id', s.id)
      
      const studentCount = s.enrollments?.[0]?.count || 0
      const attendanceRate = studentCount > 0 ? Math.round((attendanceCount / (studentCount * 10)) * 100) : 0
      
      return {
        ...s,
        course_name: s.courses?.name,
        student_count: studentCount,
        attendance_rate: Math.min(attendanceRate, 100)
      }
    }))

    return {
      lecturerName: lecturer?.full_name,
      sections: sectionsWithStats
    }
  },

  /**
   * Fetches the full roster for a specific section, including current attendance status
   */
  async getSectionRoster(sectionId) {
    const { data: enrolledStudents, error: enrolledError } = await supabase
      .from('enrollments')
      .select('student_id, students(id, full_name, student_number)')
      .eq('section_id', sectionId)

    if (enrolledError) throw enrolledError

    const { data: attendanceData, error: attendanceError } = await supabase
      .from('attendance_logs')
      .select('student_id')
      .eq('section_id', sectionId)
      .gte('session_date', new Date().toISOString().split('T')[0])

    if (attendanceError) throw attendanceError

    const attendedIds = new Set(attendanceData?.map(a => a.student_id) || [])

    return enrolledStudents.map(e => ({
      id: e.student_id,
      student_id: e.students?.student_number || e.student_id,
      full_name: e.students?.full_name,
      present: attendedIds.has(e.student_id)
    }))
  }
}
