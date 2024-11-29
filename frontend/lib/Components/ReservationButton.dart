import 'package:flutter/material.dart';

class ReservationButton extends StatefulWidget {
  final int availableVacancies;
  final Function(int numberOfPersons) onPressed;

  const ReservationButton({
    super.key,
    required this.availableVacancies,
    required this.onPressed,
  });

  @override
  _ReservationButtonState createState() => _ReservationButtonState();
}

class _ReservationButtonState extends State<ReservationButton> {
  int _numberOfPersons = 1;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Row(
              children: [
                IconButton(
                  icon: Icon(Icons.remove, color: Color(0xFF50C878)),
                  onPressed: _numberOfPersons > 1 ? () {
                    setState(() {
                      _numberOfPersons--;
                    });
                  } : null,
                ),
                Text(
                  _numberOfPersons == 1 
                    ? '$_numberOfPersons pessoa' 
                    : '$_numberOfPersons pessoas',
                  style: TextStyle(fontSize: 18),
                ),
                IconButton(
                  icon: Icon(Icons.add, color: Color(0xFF50C878)),
                  onPressed: _numberOfPersons < widget.availableVacancies ? () {
                    setState(() {
                      _numberOfPersons++;
                    });
                  } : null,
                ),
              ],
            ),
            Text(
              'Vagas Disponíveis: ${widget.availableVacancies}',
              style: TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
        SizedBox(width: 15),
        ElevatedButton(
          onPressed: _numberOfPersons > 0 && _numberOfPersons <= widget.availableVacancies
              ? () => widget.onPressed(_numberOfPersons)
              : null,
          style: ElevatedButton.styleFrom(
            backgroundColor: Color(0xFF50C878),
            foregroundColor: Colors.white,
            padding: EdgeInsets.symmetric(vertical: 15, horizontal: 20),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(30),
            ),
            elevation: 5,
          ),
          child: Text(
            'Reserve Agora',
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
      ],
    );
  }
}
