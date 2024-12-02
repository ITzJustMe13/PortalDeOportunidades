import 'package:flutter/material.dart';

class OpportunityDateAndTimePicker extends StatelessWidget {
  final DateTime? initialDate;
  final TimeOfDay? initialTime;
  final ValueChanged<DateTime> onDateTimeSelected;
  final ValueChanged<TimeOfDay> onTimeSelected;

  OpportunityDateAndTimePicker({
    Key? key,
    this.initialDate,
    this.initialTime,
    required this.onDateTimeSelected,
    required this.onTimeSelected,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        ElevatedButton(
          onPressed: () async {
            final DateTime? pickedDate = await showDatePicker(
              context: context,
              initialDate: initialDate ?? DateTime.now(),
              firstDate: DateTime.now(),
              lastDate: DateTime(2100),
            );

            if (pickedDate != null) {
              final TimeOfDay? pickedTime = await showTimePicker(
                context: context,
                initialTime: initialTime ?? TimeOfDay.now(),
              );

              if (pickedTime != null) {
                onDateTimeSelected(pickedDate);
                onTimeSelected(pickedTime);
              }
            }
          },
          child: Text(
            initialDate == null
                ? 'Selecione a Data e Hora da Oportunidade'
                : '${initialDate!.day}/${initialDate!.month}/${initialDate!.year} ${initialTime?.hour}:${initialTime?.minute}',
          ),
        ),
      ],
    );
  }
}
