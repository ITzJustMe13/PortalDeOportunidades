import 'package:flutter/material.dart';
import 'package:frontend/Enums/Location.dart'; // Import the enum for location

class OpportunityAdditionalInfo extends StatelessWidget {
  final double price;
  final int vacancies;
  final String firstName;
  final String lastName;
  final String time;
  final DateTime date;
  final Location location;
  final String address;
  

  // Constructor to accept values as parameters
  const OpportunityAdditionalInfo({
    super.key,
    required this.price,
    required this.vacancies,
    required this.firstName,
    required this.lastName,
    required this.time,
    required this.date,
    required this.location,
    required this.address,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(height: 20),
        Text(
          'Detalhes Adicionais',
          style: TextStyle(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: Colors.green,
          ),
        ),
        SizedBox(height: 10),
        Text(
          'Criado por: $firstName $lastName',
          style: TextStyle(fontSize: 16, color: Colors.black87),
        ),
        SizedBox(height: 10),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Preço: $price € / Pessoa',
              style: TextStyle(fontSize: 16, color: Colors.green),
            ),
            Text(
              '$vacancies Vagas Disponíveis',
              style: TextStyle(fontSize: 16, color: Colors.redAccent),
            ),
          ],
        ),
        SizedBox(height: 10),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Data: ${date.day}/${date.month}/${date.year}',
              style: TextStyle(fontSize: 16, color: Colors.black87),
            ),
            Text(
              'Horário: $time',
              style: TextStyle(fontSize: 16, color: Colors.black87),
            ),
          ],
        ),
        SizedBox(height: 16),
        Divider(color: Colors.grey),
        SizedBox(height: 10),
        Text(
          'Localização:',
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
        ),
        Text(
          location.name,
          style: TextStyle(fontSize: 16, color: Colors.black54),
        ),
        Text(
          'Endereço: $address',
          style: TextStyle(fontSize: 16, color: Colors.black54),
        ),
        SizedBox(height: 10),
        Text(
          'Mapa:',
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
        ),
      ],
    );
  }
}
