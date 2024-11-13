class Review {
  final int reservationId;
  final double rating;
  final String? description;

  Review({required this.reservationId, required this.rating, this.description});

  factory Review.fromJson(Map<String, dynamic> json) => Review(
      reservationId: json["reservationId"],
      rating: json["rating"],
      description: json["desc"]);

  Map<String, dynamic> toJson() =>
      {"reservationId": reservationId, "rating": rating, "desc": description};
}
