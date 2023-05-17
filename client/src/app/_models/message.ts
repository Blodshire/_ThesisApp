export interface Message {
  id: number
  senderId: number
  senderLoginName: string
  senderPhotoUrl: string
  recipientId: number
  recipientLoginName: string
  recipientPhotoUrl: string
  content: string
  dateRead: Date
  messageSent: Date
}
