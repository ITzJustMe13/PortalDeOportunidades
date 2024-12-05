import 'package:flutter/material.dart';
import 'package:frontend/Components/StarRating.dart';

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
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                // Use the StarRating widget here
                StarRating(rating: rating),
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

}
