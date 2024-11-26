import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/State/RegisterState.dart';
import 'package:provider/provider.dart';
import 'package:image_picker/image_picker.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  DateTime? _birthDateController;
  final _cellPhoneNumberController = TextEditingController();
  Gender? _genderController;
  final _IBANController = TextEditingController();
  String? _imageFile; // <--- NEW VARIABLE FOR STORED IMAGE

  final ImagePicker _picker =
      ImagePicker(); // <--- NEW INSTANCE FOR IMAGE PICKER

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _firstNameController.dispose();
    _lastNameController.dispose();
    _birthDateController = null;
    _cellPhoneNumberController.dispose();
    _genderController = null;
    _IBANController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: Consumer<RegisterState>(
        builder: (context, registerState, child) {
          return Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                TextField(
                  controller: _firstNameController,
                  decoration: InputDecoration(labelText: 'Primeiro Nome*'),
                ),
                TextField(
                  controller: _lastNameController,
                  decoration: InputDecoration(labelText: 'Último Nome*'),
                ),
                ElevatedButton(
                  child: Text(_birthDateController == null
                      ? 'Select Birth Date*'
                      : '${_birthDateController!.day}/${_birthDateController!.month}/${_birthDateController!.year}'),
                  onPressed: () async {
                    final DateTime? picked = await showDatePicker(
                      context: context,
                      initialDate: DateTime.now(),
                      firstDate: DateTime(1900),
                      lastDate: DateTime.now(),
                    );
                    if (picked != null) {
                      setState(() {
                        _birthDateController = picked;
                        print(_birthDateController);
                      });
                    }
                  },
                ),
                TextField(
                  controller: _cellPhoneNumberController,
                  decoration: InputDecoration(labelText: 'Número de Telefone*'),
                  keyboardType: TextInputType.phone,
                ),
                TextField(
                  controller: _IBANController,
                  decoration: InputDecoration(labelText: 'IBAN'),
                  keyboardType: TextInputType.text,
                ),
                DropdownButton<Gender>(
                  value: _genderController,
                  hint: Text("Select Category"),
                  onChanged: (Gender? newValue) {
                    setState(() {
                      _genderController = newValue;
                    });
                  },
                  isExpanded: true,
                  items: Gender.values.map((Gender gender) {
                    return DropdownMenuItem<Gender>(
                      value: gender,
                      child: Text(
                        gender.name.replaceAll('_', ' '),
                        overflow: TextOverflow.ellipsis,
                        maxLines: 2,
                      ),
                    );
                  }).toList(),
                  selectedItemBuilder: (BuildContext context) {
                    return Gender.values.map((Gender gender) {
                      return Text(
                        gender.name.replaceAll('_', ' '),
                        overflow: TextOverflow.ellipsis,
                        maxLines: 1,
                      );
                    }).toList();
                  },
                ),
                TextField(
                  controller: _emailController,
                  decoration: InputDecoration(labelText: 'Email'),
                  keyboardType: TextInputType.emailAddress,
                ),
                SizedBox(height: 16.0),
                TextField(
                  controller: _passwordController,
                  decoration: InputDecoration(labelText: 'Password'),
                  obscureText: true,
                ),
                ElevatedButton(
                  child: Text(
                      _imageFile == null ? 'Pick Image' : 'Image Selected'),
                  onPressed: () async {
                    final XFile? pickedFile =
                        await _picker.pickImage(source: ImageSource.gallery);
                    if (pickedFile != null) {
                      final Uint8List fileBytes =
                          await pickedFile.readAsBytes(); // Read as bytes
                      final String base64Image = base64Encode(fileBytes);
                      setState(() {
                        _imageFile = base64Image;
                        print(_imageFile);
                      });
                    }
                  },
                ),
                SizedBox(height: 24.0),
                if (registerState.isLoading)
                  CircularProgressIndicator()
                else
                  ElevatedButton(
                    onPressed: () async {
                      _registerUser(registerState);
                    },
                    child: Text('Registar-me'),
                  ),
                SizedBox(height: 16.0),
                if (registerState.errorMessage != null)
                  Text(
                    registerState.errorMessage!,
                    style: TextStyle(color: Colors.red),
                  ),
              ],
            ),
          );
        },
      ),
    );
  }

  _registerUser(RegisterState registerState) {
    final email = _emailController.text;
    final password = _passwordController.text;
    final firstName = _firstNameController.text;
    final lastName = _lastNameController.text;
    final birthDate = _birthDateController;
    final cellPhoneNumber = _cellPhoneNumberController.text;
    final gender = _genderController;
    final IBAN = _IBANController.text;
    final image = _imageFile;

    if (_imageFile != null) {
      final user = User(
          userId: 0,
          password: password,
          firstName: firstName,
          lastName: lastName,
          birthDate: birthDate ?? DateTime.now(),
          registrationDate: DateTime.now(),
          email: email,
          cellPhoneNumber: int.parse(cellPhoneNumber),
          gender: gender ?? Gender.values.first,
          IBAN: IBAN,
          image: image ?? "");

      registerState.register(user);
    }
  }
}
