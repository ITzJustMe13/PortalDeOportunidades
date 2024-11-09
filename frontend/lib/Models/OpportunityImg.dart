class OpportunityImg {
  final int imgId;
  final int opportunityId;
  final String imageBase64; // Store the image as a base64 string

  OpportunityImg({
    required this.imgId,
    required this.opportunityId,
    required this.imageBase64,
  });

  // Factory method to create an instance from JSON data
  factory OpportunityImg.fromJson(Map<String, dynamic> json) {
    return OpportunityImg(
      imgId: json['imgId'],
      opportunityId: json['opportunityId'],
      imageBase64: json['image'], // Backend sends the image as a base64 string
    );
  }
}