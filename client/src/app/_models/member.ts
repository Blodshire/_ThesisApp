import { Photo } from "./photo"



export interface Member {
  id: number
  loginName: string
  photoUrl: string
  age: number
  userName: any
  created: string
  lastActive: string
  gender: string
  introduction: string
  lookingFor: string
  interests: string
  city: string
  country: string
  photos: Photo[]
}

