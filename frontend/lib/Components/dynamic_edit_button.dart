import 'package:flutter/material.dart';

class DynamicEditButton extends StatelessWidget {
  final void Function()? onPressed;

  const DynamicEditButton({
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
            color: Colors.green[900],
            onPressed: onPressed,
            icon: Icon(Icons.info),
          );
        } else {
          return TextButton(
            onPressed: onPressed,
            style: TextButton.styleFrom(
              backgroundColor: Colors.green[900],
              padding: EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            ),
            child: Text(
              "Editar",
              style: TextStyle(fontSize: 12, color: Colors.white),
            ),
          );
        }
      },
    );
  }
}
