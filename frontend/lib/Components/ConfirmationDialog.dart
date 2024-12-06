import 'package:flutter/material.dart';

class ConfirmationDialog extends StatelessWidget {
  final String action;
  final String message;
  final Future<bool> Function() onConfirm;

  const ConfirmationDialog({
    super.key,
    required this.action,
    required this.onConfirm,
    required this.message,
  });

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text('Confirmar $action'),
      content: Text(message),
      actions: <Widget>[
        TextButton(
          onPressed: () {
            Navigator.of(context).pop(false); // User canceled
          },
          child: Text('Cancelar'),
        ),
        TextButton(
          onPressed: () async {
            final bool confirmed =
                await onConfirm(); // Call the onConfirm function
            Navigator.of(context).pop(confirmed);
          },
          child: Text('Confirmar'),
        ),
      ],
    );
  }
}
