import 'package:flutter/material.dart';

class ReviewCard extends StatelessWidget {
  final double rating;
  final String description;

  const ReviewCard({
    super.key,
    required this.rating,
    required this.description,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 5,
      margin: const EdgeInsets.all(10),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10),
      ),
      child: Padding(
        padding: const EdgeInsets.all(15.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Row for username and rating stars
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                // Display the star rating
                Row(
                  children: buildStarRating(rating),
                ),
              ],
            ),
            const SizedBox(height: 10),
            // Display the review text
            Text(
              description,
              style: TextStyle(fontSize: 16),
            ),
          ],
        ),
      ),
    );
  }

  // Function to build the star rating based on the double value
  List<Widget> buildStarRating(double rating) {
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
