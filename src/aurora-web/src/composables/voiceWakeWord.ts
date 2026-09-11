export type WakeWordMatch = {
  activated: boolean
  command: string
}

const WAKE_WORD_PATTERN = /^\s*aurora\b[\s,;:!?\-—]*?/i

export function extractAuroraCommand(transcript: string): WakeWordMatch {
  const trimmed = transcript.trim()

  if (!trimmed) {
    return { activated: false, command: '' }
  }

  if (!/^\s*aurora\b/i.test(trimmed)) {
    return { activated: false, command: '' }
  }

  return {
    activated: true,
    command: trimmed.replace(WAKE_WORD_PATTERN, '').trim(),
  }
}
