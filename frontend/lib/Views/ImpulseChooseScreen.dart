import 'package:flutter/material.dart';

class ImpulseChooseScreen extends StatelessWidget {
  const ImpulseChooseScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Impulsos'),
      ),
      body: Center(
        child: Wrap(children: [
          _buildImpulseCard(7),
          SizedBox(height: 20),
          _buildImpulseCard(30),
          SizedBox(height: 20),
          _buildImpulseCard(60),
        ]),
      ),
    );
  }
}

Widget _buildImpulseCard(int days) {
  return Card(
    child: ListTile(
      title: Text('Impulso de $days dias'),
      onTap: () {
        //
      },
    ),
  );
}
