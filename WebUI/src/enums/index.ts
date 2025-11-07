export enum GenerationType {
  TextToImage,
  ImageToImage,
}

export enum GenerationStatus
{
  Pending,
  Processing,
  Completed,
  Failed,
  Cancelled,
}

export enum UserRole
{
  User,
  PowerUser,
  Admin,
  Owner,
}

export enum TransactionType
{
  Recharge,
  Consume,
  Earn,
  Refund,
  Transfer,
}
