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
    Cancelled 
}