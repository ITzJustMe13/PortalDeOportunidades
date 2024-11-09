import 'package:frontend/Enums/Gender.dart';

class User {
  final int userId;
  final String password;
  final String firstName;
  final String lastName;
  final String email;
  final int cellPhoneNumber;
  final DateTime registrationDate;
  final DateTime birthDate;
  final Gender gender;
  final String image;
  final String? IBAN;

  const User({
    required this.userId,
    required this.password,
    required this.firstName,
    required this.lastName,
    required this.birthDate,
    required this.registrationDate,
    required this.email,
    required this.cellPhoneNumber,
    required this.gender,
    this.IBAN,
    required this.image
  });

  factory User.fromJson(Map<String, dynamic> json) => User(
    userId: json["userId"],
    password: json["password"],
    firstName: json["firstName"],
    lastName: json["lastName"],
    birthDate: DateTime.parse(json["birthDate"]),
    registrationDate: DateTime.parse(json["registrationDate"]),
    email: json["email"],
    cellPhoneNumber: json["cellPhoneNumber"],
    gender: genderFromInt(json["gender"]),
    IBAN: json["IBAN"],
    image: json["image"],
  );

  Map<String, dynamic> toJson()=>{
    "userId": userId,
    "password": password,
    "firstName": firstName,
    "lastName": lastName,
    "birthDate": birthDate.toIso8601String(),
    "registrationDate": registrationDate.toIso8601String(),
    "email": email,
    "cellPhoneNumber": cellPhoneNumber,
    "gender" : genderToInt(gender),
    "IBAN" : IBAN,
    "image" : image
  };
}