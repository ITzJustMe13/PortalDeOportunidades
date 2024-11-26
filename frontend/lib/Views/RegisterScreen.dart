import 'dart:convert';
import 'dart:io';
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
  String? _imageBase64;
  final _formKey = GlobalKey<FormState>();

  String _errorMessage = '';

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
            child: Form(
              // Wrap with Form
              key: _formKey,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  TextFormField(
                    // Use TextFormField for validation
                    controller: _firstNameController,
                    decoration:
                        const InputDecoration(labelText: 'Primeiro Nome*'),
                    validator: (value) {
                      if (value!.isEmpty) {
                        return 'Por favor insira o seu primeiro nome';
                      }
                      return null;
                    },
                  ),
                  TextFormField(
                    controller: _lastNameController,
                    decoration:
                        const InputDecoration(labelText: 'Último Nome*'),
                    validator: (value) {
                      if (value!.isEmpty) {
                        return 'Por favor insira o seu último nome';
                      }
                      return null;
                    },
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
                        });
                      }
                    },
                  ),
                  TextFormField(
                    controller: _cellPhoneNumberController,
                    decoration:
                        const InputDecoration(labelText: 'Número de Telefone*'),
                    keyboardType: TextInputType.phone,
                    validator: (value) {
                      if (value!.isEmpty || value.length < 9) {
                        return 'Por favor insira um número de telefone válido';
                      }
                      return null;
                    },
                  ),
                  /*TODO - adicionar validação para IBAN*/
                  TextFormField(
                    controller: _IBANController,
                    decoration: InputDecoration(
                      labelText: 'IBAN',
                      suffixText: '(Opcional)',
                    ),
                    keyboardType: TextInputType.text,
                    validator: (value) {
                      if (value != null &&
                          value.isNotEmpty &&
                          value.length < 9) {
                        return 'Por favor insira um IBAN com pelo menos 9 caracteres';
                      }
                      return null;
                    },
                  ),
                  DropdownButtonFormField<Gender>(
                    value: _genderController,
                    hint: Text("Select Category*"),
                    onChanged: (Gender? newValue) {
                      setState(() {
                        _genderController = newValue;
                      });
                    },
                    validator: (value) {
                      if (value == null) {
                        return 'Por favor selecione um género';
                      }
                      return null;
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
                  TextFormField(
                    controller: _emailController,
                    decoration: const InputDecoration(labelText: 'Email*'),
                    keyboardType: TextInputType.emailAddress,
                    validator: (value) {
                      if (value!.isEmpty || !value.contains('@')) {
                        return 'Por favor insira um email válido';
                      }
                      return null;
                    },
                  ),
                  SizedBox(height: 16.0),
                  TextFormField(
                    controller: _passwordController,
                    decoration: const InputDecoration(labelText: 'Password*'),
                    obscureText: true,
                    validator: (value) {
                      if (value!.isEmpty || value.length < 8) {
                        return 'A palavra-passe deve ter no mínimo 8 caracteres';
                      }
                      return null;
                    },
                  ),
                  SizedBox(height: 16.0),
                  ElevatedButton(
                    child: Text(_imageBase64 == null
                        ? 'Escolha uma imagem de Perfil'
                        : 'Imagem Selecionada'),
                    onPressed: () async {
                      final XFile? pickedFile =
                          await _picker.pickImage(source: ImageSource.gallery);
                      if (pickedFile != null) {
                        final Uint8List fileBytes =
                            await pickedFile.readAsBytes(); // Read as bytes
                        final String base64Image = base64Encode(fileBytes);
                        setState(() {
                          _imageBase64 = base64Image;
                        });
                      }
                    },
                  ),
                  SizedBox(height: 16.0),
                  Center(
                    child: _imageBase64 != null
                        ? Image.memory(
                            width: 50,
                            height: 50,
                            Uint8List.fromList(base64Decode(_imageBase64!)),
                          )
                        : Text('Nenhuma imagem selecionada'),
                  ),
                  SizedBox(height: 24.0),
                  if (registerState.isLoading)
                    CircularProgressIndicator()
                  else
                    ElevatedButton(
                      onPressed: () async {
                        if (_formKey.currentState!.validate()) {
                          _registerUser(registerState);
                        }
                      },
                      child: Text('Registar-me'),
                    ),
                  SizedBox(height: 16.0),
                  if (_errorMessage.isNotEmpty)
                    Text(
                      _errorMessage,
                      style: TextStyle(color: Colors.red),
                    ),
                ],
              ),
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
    final image = _imageBase64;

    if (image == null || image.isEmpty) {
      setState(() {
        _errorMessage = 'Por favor escolha uma imagem de perfil';
      });
      return;
    } else if (birthDate == null || birthDate == DateTime.now()) {
      setState(() {
        _errorMessage = 'Por favor selecione uma data de nascimento';
      });
      return;
    }

    if (_imageBase64 != null) {
      final user = User(
          userId: 0,
          password: password,
          firstName: firstName,
          lastName: lastName,
          birthDate: birthDate,
          registrationDate: DateTime.now(),
          email: email,
          cellPhoneNumber: int.parse(cellPhoneNumber),
          gender: gender ?? Gender.values.first,
          IBAN: IBAN,
          image: image);

      registerState.register(user, context);
    }
  }
}
