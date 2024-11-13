class Reservation {
  final int? reservationId;
  final int opportunityId;
  final int userId;
  final DateTime reservationDate;
  final DateTime checkInDate;
  final int numOfPeople;
  final bool isActive;
  final double fixedPrice;

  Reservation(
      {this.reservationId,
      required this.opportunityId,
      required this.userId,
      required this.reservationDate,
      required this.checkInDate,
      required this.numOfPeople,
      required this.isActive,
      required this.fixedPrice});

  factory Reservation.fromJson(Map<String, dynamic> json) => Reservation(
      reservationId: json["reservationId"],
      opportunityId: json["opportunityId"],
      userId: json["userId"],
      reservationDate: DateTime.parse(json["reservationDate"]),
      checkInDate: DateTime.parse(json["checkInDate"]),
      numOfPeople: json["numOfPeople"],
      isActive: json["isActive"],
      fixedPrice: json["fixedPrice"]);

  Map<String, dynamic> toJson() => {
        "opportunityId": opportunityId,
        "userId": userId,
        "numOfPeople": numOfPeople,
      };
}
