export interface NextEvent {
  title: string
  time: string
}

export interface AuroraStatus {
  status: string
}

export interface AgendaSummary {
  eventsToday: number
  nextEvent: NextEvent | null
}

export interface TasksSummary {
  pending: number
}

export interface EmailsSummary {
  important: number
}

export interface SystemStatus {
  api: string
  hermes: string
  memory: string
}

export interface DashboardResponse {
  greeting: string
  aurora: AuroraStatus
  agenda: AgendaSummary
  tasks: TasksSummary
  emails: EmailsSummary
  system: SystemStatus
}
