import 'package:flutter/material.dart';

class StarRatingSelector extends StatefulWidget {
  final int maxRating; // Maximum number of stars
  final double currentRating; // Current rating value
  final ValueChanged<double> onRatingChanged; // Callback to notify parent of rating change
  final double iconSize; // Size of the star icons
  final Color selectedColor; // Color of selected stars
  final Color unselectedColor; // Color of unselected stars

  const StarRatingSelector({
    super.key,
    this.maxRating = 5,
    this.currentRating = 0,
    required this.onRatingChanged,
    this.iconSize = 30.0,
    this.selectedColor = Colors.amber,
    this.unselectedColor = Colors.grey,
  });

  @override
  _StarRatingSelectorState createState() => _StarRatingSelectorState();
}

class _StarRatingSelectorState extends State<StarRatingSelector> {
  late double currentRating;

  @override
  void initState() {
    super.initState();
    currentRating = widget.currentRating;
  }

  void _handleTap(int index) {
    setState(() {
      currentRating = index.toDouble();
    });
    widget.onRatingChanged(currentRating);
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: List.generate(widget.maxRating, (index) {
        return IconButton(
          icon: Icon(
            index < currentRating ? Icons.star : Icons.star_border,
            color: index < currentRating
                ? widget.selectedColor
                : widget.unselectedColor,
          ),
          iconSize: widget.iconSize,
          onPressed: () => _handleTap(index + 1), // Select the rating
        );
      }),
    );
  }
}