class Favorite{
  final int userId
  final int opportunityId

  Favorite({
    required this.userId,
    required this.opportunityId
  })

  factory Favorite.fromJson(Map<String, dynamic> json) => Favorite{
    userId: json["userId"],
    opportunityId: json["opportunityId"]
  }

  Map<String, dynamic> toJson() =>{
    "userId": userId,
    "opportunityId": opportunityId
  }

}