import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Enums/Gender.dart';
import '../Models/User.dart';

class EditProfileScreen extends StatefulWidget {
  const EditProfileScreen({super.key});

  @override
  _EditProfileScreenState createState() => _EditProfileScreenState();
}

class _EditProfileScreenState extends State<EditProfileScreen> {
  User user = User(
    userId: 1,
    firstName: "Antonio",
    lastName: "Silva",
    email: "antonio.silva@gmail.com",
    password: "123456789",
    birthDate: DateTime(2004),
    registrationDate: DateTime.now(),
    cellPhoneNumber: 911232938,
    gender: Gender.MASCULINO,
    image: "https://via.placeholder.com/150",
  );

  TextEditingController emailController = TextEditingController();
  TextEditingController phoneController = TextEditingController();

  @override
  void initState() {
    super.initState();
    emailController.text = user.email;
    phoneController.text = user.cellPhoneNumber.toString();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Editar Perfil'),
      ),
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
        Container(
          width: 100,
          height: 100,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(8.0),
            image: DecorationImage(
              image: NetworkImage(user.image),
              fit: BoxFit.cover,
            ),
          ),
        ),
        const SizedBox(height: 8),
        ElevatedButton(
          onPressed: () {
            // Lógica para escolher nova foto
          },
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.green,
          ),
          child: const Text('Escolher Foto'),
        ),
      ],
    );
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
  void _saveChanges() {
    // Validação de email e telefone
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

    // Atualização do estado
    setState(() {
      // Criar uma nova instância do User com as alterações
      user = User(
        userId: user.userId,
        firstName: user.firstName,
        lastName: user.lastName,
        email: emailController.text,
        password: user.password,
        birthDate: user.birthDate,
        registrationDate: user.registrationDate,
        cellPhoneNumber: phoneNumber,
        gender: user.gender,
        image: user.image,
      );
    });

    // Feedback ao usuário
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Alterações gravadas com sucesso!'),
        backgroundColor: Colors.green,
      ),
    );
  }
}
