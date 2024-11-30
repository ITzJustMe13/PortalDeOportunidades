import 'package:flutter/material.dart';

class ConfirmationDialog extends StatelessWidget {
  final String action;
  final Future<bool> Function() onConfirm;

  const ConfirmationDialog({
    Key? key,
    required this.action,
    required this.onConfirm,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text('Confirmar $action'),
      content: Text('Tem certeza de que deseja $action esta oportunidade?'),
      actions: <Widget>[
        TextButton(
          onPressed: () {
            Navigator.of(context).pop(false); // User canceled
          },
          child: Text('Cancelar'),
        ),
        TextButton(
          onPressed: () async {
            final bool confirmed = await onConfirm(); // Call the onConfirm function
            Navigator.of(context).pop(confirmed);
          },
          child: Text('Confirmar'),
        ),
      ],
    );
  }
}
