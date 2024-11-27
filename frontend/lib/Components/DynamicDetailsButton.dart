import 'package:flutter/material.dart';

class DynamicDetailsButton extends StatelessWidget {
  final void Function()? onPressed;

  const DynamicDetailsButton({
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
            color: Color(0xFF50C878),
            onPressed: onPressed,
            icon: Icon(Icons.info),
          );
        } else {
          return TextButton(
            onPressed: onPressed,
            style: TextButton.styleFrom(
              backgroundColor: Color(0xFF50C878),
              padding: EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            ),
            child: Text(
              "Detalhes",
              style: TextStyle(fontSize: 12, color: Colors.white),
            ),
          );
        }
      },
    );
  }
}
