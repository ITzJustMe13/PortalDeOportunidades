import 'package:flutter/material.dart';

class StarRating extends StatelessWidget {
  final double rating;

  const StarRating({
    super.key,
    required this.rating,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min, // To ensure it doesn't take extra space
      children: _buildStarRating(),
    );
  }

  List<Widget> _buildStarRating() {
    List<Widget> stars = [];
    // Full stars
    for (int i = 0; i < rating.floor(); i++) {
      stars.add(const Icon(Icons.star, color: Colors.amber, size: 20));
    }
    // Half star if needed
    if (rating - rating.floor() >= 0.5) {
      stars.add(const Icon(Icons.star_half, color: Colors.amber, size: 20));
    }
    // Empty stars
    while (stars.length < 5) {
      stars.add(const Icon(Icons.star_border, color: Colors.amber, size: 20));
    }
    return stars;
  }
}
