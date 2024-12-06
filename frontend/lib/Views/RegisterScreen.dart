import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
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
  final _ibanController = TextEditingController();
  String? _imageBase64;
  final _formKey = GlobalKey<FormState>();

  String _errorMessage = '';

  final ImagePicker _picker = ImagePicker();

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _firstNameController.dispose();
    _lastNameController.dispose();
    _birthDateController = null;
    _cellPhoneNumberController.dispose();
    _genderController = null;
    _ibanController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: Consumer<RegisterState>(
        builder: (context, registerState, child) {
          return LayoutBuilder(
            builder: (context, constraints) {
              double screenWidth = constraints.maxWidth;

              double componentWidth = screenWidth > 1200
                  ? screenWidth * 0.4
                  : (screenWidth > 800 ? screenWidth * 0.6 : screenWidth * 1);
              return SingleChildScrollView(
                padding: const EdgeInsets.all(16.0),
                child: registerState.isActivationSuccess
                    ? _buildAccountNotActivated()
                    : _buildForm(registerState, componentWidth),
              );
            },
          );
        },
      ),
    );
  }

  Widget _buildForm(RegisterState registerState, double componentWidth) {
    return Form(
      key: _formKey,
      child: Center(
        child: SizedBox(
          width: componentWidth,
          child: Card(
            elevation: 4,
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: _buildFormItems(registerState, componentWidth),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildFormItems(RegisterState registerState, double componentWidth) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Text("Registe-se",
            style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: Colors.black)),
        SizedBox(height: 16.0),
        TextFormField(
          // Use TextFormField for validation
          controller: _firstNameController,
          decoration: const InputDecoration(labelText: 'Primeiro Nome*'),
          validator: (value) {
            if (value!.isEmpty) {
              return 'Por favor insira o seu primeiro nome';
            }
            return null;
          },
        ),
        TextFormField(
          controller: _lastNameController,
          decoration: const InputDecoration(labelText: 'Último Nome*'),
          validator: (value) {
            if (value!.isEmpty) {
              return 'Por favor insira o seu último nome';
            }
            return null;
          },
        ),
        SizedBox(height: 16.0),
        ElevatedButton(
          child: Text(_birthDateController == null
              ? 'Selecione a Data de Nascimento*'
              : '${_birthDateController!.day}/${_birthDateController!.month}/${_birthDateController!.year}'),
          onPressed: () async {
            final DateTime today = DateTime.now();
            final DateTime eighteenYearsAgo =
                DateTime(today.year - 18, today.month, today.day);
            final DateTime? picked = await showDatePicker(
              context: context,
              initialDate: eighteenYearsAgo,
              firstDate: DateTime(1900),
              lastDate: eighteenYearsAgo,
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
          decoration: const InputDecoration(labelText: 'Número de Telefone*'),
          keyboardType: TextInputType.phone,
          validator: (value) {
            if (value!.isEmpty || value.length < 9) {
              return 'Por favor insira um número de telefone válido';
            }
            return null;
          },
        ),
        TextFormField(
          controller: _ibanController,
          decoration: InputDecoration(
            labelText: 'IBAN',
            suffixText: '(Opcional)',
          ),
          keyboardType: TextInputType.text,
          validator: (value) {
            if (value != null && value.isNotEmpty && value.length < 9) {
              return 'Por favor insira um IBAN com pelo menos 9 caracteres';
            }
            return null;
          },
        ),
        DropdownButtonFormField<Gender>(
          value: _genderController,
          hint: Text("Selecione Gênero*"),
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
          DynamicActionButton(
            text: 'Registar-me',
            icon: Icons.account_circle,
            color: Color(0xFF50C878),
            onPressed: () async {
              if (_formKey.currentState!.validate()) {
                await _registerUser(registerState);
              }
            },
          ),
        SizedBox(height: 16.0),
        if (_errorMessage.isNotEmpty)
          Text(
            _errorMessage,
            style: TextStyle(color: Colors.red),
          ),
      ],
    );
  }

  Widget _buildAccountNotActivated() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.error_outline,
              color: Colors.green,
              size: 100,
            ),
            SizedBox(height: 30),
            Text(
              'Envio de Link de Ativação',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 30),
            Text(
              'A sua conta não está ativa, por favor verifique a sua caixa de correio eletrónico pelo link de ativação de conta (verifique a caixa de spam). \n\nApenas terá acesso a todas as funcionalidades do Portal de Oportunidades após a ativação da conta.',
            ),
            SizedBox(height: 30),
            DynamicActionButton(
              text: "Home",
              icon: Icons.home,
              color: Color(0xFF50C878),
              onPressed: () async {
                await Navigator.pushNamed(context, "/");
              },
            ),
          ],
        ),
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
    final iban = _ibanController.text;
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
          iban: iban,
          image: image);

      registerState.register(user, context);
    }
  }
}
