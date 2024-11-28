import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Gender.dart';
import '../Models/User.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:image_picker/image_picker.dart';
import 'dart:convert';
import 'dart:typed_data';

class EditProfileScreen extends StatefulWidget {

  final User user;

  const EditProfileScreen({super.key, required this.user});

  @override
  _EditProfileScreenState createState() => _EditProfileScreenState();
}

class _EditProfileScreenState extends State<EditProfileScreen> {
  

  TextEditingController emailController = TextEditingController();
  TextEditingController phoneController = TextEditingController();
  final ImagePicker _picker = ImagePicker();
  String? _updatedImageBase64;

  @override
  void initState() {
    super.initState();
    emailController.text = widget.user.email;
    phoneController.text = widget.user.cellPhoneNumber.toString();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          _buildProfileImage(),
          const Divider(height: 32),
          _buildEmailField(),
          const SizedBox(height: 16),
          _buildPhoneField(),
          const SizedBox(height: 16),
          _buildSaveButton(),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 64.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildProfileImage(),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildEmailField(),
                const Divider(height: 32),
                _buildPhoneField(),
                const SizedBox(height: 16),
                _buildSaveButton(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDesktopLayout() {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 200.0, vertical: 50.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Imagem do perfil
          _buildProfileImage(),
          const SizedBox(width: 32),
          // Campos de entrada e botões
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Expanded(child: _buildEmailField()), // Campo de email
                    const SizedBox(width: 16), // Espaçamento
                    _buildSaveButton(), // Botão de salvar
                  ],
                ),
                const Divider(height: 32, thickness: 1), // Divisor
                _buildPhoneField(), // Campo de telefone
              ],
            ),
          ),
        ],
      ),
    );
  }

  // Widget para exibir a imagem do perfil com botão
 Widget _buildProfileImage() {
  return Column(
    children: [
      // Display the profile image or placeholder
      _buildImage(_updatedImageBase64 ?? widget.user.image),
      const SizedBox(height: 8),
      ElevatedButton(
        onPressed: () async {
          // Picking the image from the gallery
          final XFile? pickedFile = await _picker.pickImage(source: ImageSource.gallery);
          if (pickedFile != null) {
            // Convert the image to bytes and then encode to base64
            final Uint8List fileBytes = await pickedFile.readAsBytes();
            final String base64Image = base64Encode(fileBytes);

            // Store the new base64 image temporarily
            setState(() {
              _updatedImageBase64 = base64Image;
            });
          }
        },
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.green,
        ),
        child: const Text('Escolher Foto'),
      ),
    ],
  );
}

// This is the helper function to display the image or placeholder
Widget _buildImage(String image) {
  if (image.isNotEmpty) {
    // Decode the Base64 string into bytes
    final decodedBytes = base64Decode(image);

    return Container(
      width: 150,
      height: 150,
      decoration: BoxDecoration(
        color: Colors.grey[300],
        borderRadius: BorderRadius.circular(8.0),
        image: DecorationImage(
          image: MemoryImage(decodedBytes), // Use MemoryImage to decode the Base64
          fit: BoxFit.cover,
        ),
      ),
    );
  } else {
    // If no image is provided, display a placeholder icon
    return Container(
      width: 150,
      height: 150,
      decoration: BoxDecoration(
        color: Colors.grey[300],
        borderRadius: BorderRadius.circular(8.0),
      ),
      child: Icon(
        Icons.person,
        size: 80,
        color: Colors.grey[600],
      ),
    );
  }
}

  // Widget para o campo de email
  Widget _buildEmailField() {
    return Row(
      children: [
        const Text("Email:"),
        const SizedBox(width: 8),
        Expanded(
          child: TextField(
            controller: emailController,
            decoration: const InputDecoration(
              border: OutlineInputBorder(),
              isDense: true,
            ),
          ),
        ),
      ],
    );
  }

  // Widget para o campo de número de telefone
  Widget _buildPhoneField() {
    return Row(
      children: [
        const Text("Número de Telemovel:"),
        const SizedBox(width: 8),
        Expanded(
          child: TextField(
            controller: phoneController,
            decoration: const InputDecoration(
              border: OutlineInputBorder(),
              isDense: true,
            ),
          ),
        ),
      ],
    );
  }

  // Widget para o botão de salvar alterações
  Widget _buildSaveButton() {
    return ElevatedButton(
      onPressed: () {
        // Lógica para salvar alterações
        _saveChanges();
      },
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.green,
      ),
      child: const Text('Gravar Alterações'),
    );
  }

  // Lógica para salvar alterações
  void _saveChanges() async {
    // Validate email and phone
    if (emailController.text.isEmpty || phoneController.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Por favor, preencha todos os campos!'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    int? phoneNumber = int.tryParse(phoneController.text);
    if (phoneNumber == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Número de telemóvel inválido!'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    // Create updated user object
    User updatedUser = User(
      userId: widget.user.userId,
      firstName: widget.user.firstName,
      lastName: widget.user.lastName,
      email: emailController.text,
      password: widget.user.password,
      birthDate: widget.user.birthDate,
      registrationDate: widget.user.registrationDate,
      cellPhoneNumber: phoneNumber,
      gender: widget.user.gender,
      image: _updatedImageBase64.toString(),
    );

    final success = await Provider.of<UserApiHandler>(context, listen: false)
        .editUser(widget.user.userId, updatedUser);
  
    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alterações gravadas com sucesso!'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Falha ao gravar as alterações. Tente novamente!'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }
}
