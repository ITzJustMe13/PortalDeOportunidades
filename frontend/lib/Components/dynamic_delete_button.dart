import 'package:flutter/material.dart';

class DynamicDeleteButton extends StatelessWidget {
  final void Function()? onPressed;

  const DynamicDeleteButton({
    super.key,
    this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        // Define a threshold width for switching from text to icon
        double thresholdWidth = 100.0;

        if (constraints.maxWidth < thresholdWidth) {
          return IconButton(
            color: Colors.red,
            onPressed: onPressed,
            icon: Icon(Icons.info),
          );
        } else {
          return TextButton(
            onPressed: onPressed,
            style: TextButton.styleFrom(
              backgroundColor: Colors.red,
              padding: EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            ),
            child: Text(
              "Apagar",
              style: TextStyle(fontSize: 12, color: Colors.white),
            ),
          );
        }
      },
    );
  }
}
