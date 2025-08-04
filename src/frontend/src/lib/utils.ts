import { type ClassValue, clsx } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatTimestamp(timestamp: string): string {
  const date = new Date(timestamp)
  return date.toLocaleTimeString()
}

export function getAgentStatusColor(status: string): string {
  switch (status.toLowerCase()) {
    case 'active':
      return 'text-green-600 bg-green-100'
    case 'busy':
      return 'text-orange-600 bg-orange-100'
    case 'error':
      return 'text-red-600 bg-red-100'
    default:
      return 'text-gray-600 bg-gray-100'
  }
}

export function getAgentTypeIcon(type: string): string {
  switch (type) {
    case 'chat':
      return '💬'
    case 'evolution':
      return '🧬'
    case 'tool':
      return '🔧'
    default:
      return '🤖'
  }
}