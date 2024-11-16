import 'package:flutter/material.dart';

class ReservationButton extends StatefulWidget {
  final int availableVacancies;
  final VoidCallback onPressed; // Callback when the button is pressed

  const ReservationButton({
    Key? key,
    required this.availableVacancies,
    required this.onPressed,
  }) : super(key: key);

  @override
  _ReservationButtonState createState() => _ReservationButtonState();
}

class _ReservationButtonState extends State<ReservationButton> {
  int _numberOfPersons = 1; // Default to 1 person
  late int _availableVacancies;

  @override
  void initState() {
    super.initState();
    _availableVacancies = widget.availableVacancies;
  }

  void _increasePersons() {
    if (_numberOfPersons < _availableVacancies) {
      setState(() {
        _numberOfPersons++;
      });
    }
  }

  void _decreasePersons() {
    if (_numberOfPersons > 1) {
      setState(() {
        _numberOfPersons--;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        // Number of persons selector
        Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Row(
              children: [
                IconButton(
                  icon: Icon(Icons.remove, color: Color(0xFF50C878)),
                  onPressed: _decreasePersons,
                ),
                Text(
                  _numberOfPersons == 1 
                  ? '$_numberOfPersons pessoa' 
                  : '$_numberOfPersons pessoas',
                  style: TextStyle(fontSize: 18),
                ),
                IconButton(
                  icon: Icon(Icons.add, color: Color(0xFF50C878)),
                  onPressed: _increasePersons,
                ),
              ],
            ),
            Text(
              'Vagas Disponiveis: $_availableVacancies',
              style: TextStyle(fontSize: 14, color: Colors.grey),
            ),
          ],
        ),
        SizedBox(width: 15), // Space between the selector and the button
        // Reserve Now Button with adjusted padding
        ElevatedButton(
          onPressed: _numberOfPersons > 0 && _numberOfPersons <= _availableVacancies
              ? widget.onPressed
              : null, // Disable button if the condition is not met
          style: ElevatedButton.styleFrom(
            backgroundColor: Color(0xFF50C878), // Button color
            foregroundColor: Colors.white, // Text color
            padding: EdgeInsets.symmetric(vertical: 15, horizontal: 20), // Reduce horizontal padding slightly
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(30), // Rounded corners
            ),
            elevation: 5, // Shadow effect
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
