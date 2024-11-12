class Impulse{
  final int userId
  final int opportunityId
  final double value
  final DateTime expireDate

  Impulse({
    required this.userId,
    required this.opportunityId,
    required this.value,
    required this.expireDate
  })

  factory Impulse.fromJson(Map<String,dynamic> json) Impulse =>{
    userId: json["userId"],
    opportunityId: json["opportunityId"],
    value: json["value"],
    expireDate: DateTime.parse(json["expireDate"])
  }

  Map<String,dynamic> toJson() =>{
    "userId": userId,
    "opportunityId": opportunityId,
    "value": value
  }

}