import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Enums/Gender.dart';
import '../Models/User.dart';

class PerfilScreen extends StatefulWidget {
  @override
  _PerfilScreenState createState() => _PerfilScreenState();
}

class _PerfilScreenState extends State<PerfilScreen> {
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
      image: "teste");

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          title: const Text('Perfil'),
        ),
        body: LayoutBuilder(builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            // Layout para telas pequenas (smartphones)
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            // Layout para telas médias (tablets)
            return _buildTabletLayout();
          } else {
            // Layout para telas grandes (desktops)
            return _buildDesktopLayout();
          }
        }));
  }

  Widget _buildMobileLayout() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          _buildImage(),
          const SizedBox(height: 16),
          _buildEditProfileButton(),
          const SizedBox(height: 16),
          _buildExtraInfo(),
          const SizedBox(height: 16),
          _buildUserInfo(),
          const SizedBox(height: 16),
          _buildHistoryButton(),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildImage(),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        _buildUserInfo(),
                        _buildEditProfileButton(),
                      ],
                    ),
                    const Divider(thickness: 1, height: 32),
                    _buildExtraInfo(),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          Center(child: _buildHistoryButton()),
        ],
      ),
    );
  }

  Widget _buildDesktopLayout() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildImage(),
              const SizedBox(height: 16),
              _buildExtraInfo(),
              const SizedBox(height: 16),
              _buildHistoryButton(),
            ],
          ),
          const SizedBox(width: 24),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    _buildUserInfo(),
                    _buildEditProfileButton(),
                  ],
                ),
                const Divider(thickness: 1, height: 32),
              ],
            ),
          ),
        ],
      ),
    );
  }

  /// Método para construir o layout da imagem do perfil
  Widget _buildImage() {
    return Container(
      width: 150,
      height: 150,
      decoration: BoxDecoration(
        color: Colors.grey[300],
        borderRadius: BorderRadius.circular(8.0),
        image: DecorationImage(
          image: NetworkImage(user.image),
          fit: BoxFit.cover,
        ),
      ),
    );
  }

  /// Método para construir o botão "Editar Perfil"
  Widget _buildEditProfileButton() {
    return ElevatedButton(
      onPressed: () {
        // Ação do botão "Editar Perfil"
      },
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.green,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      ),
      child: const Text(
        'Editar Perfil',
        style: TextStyle(color: Colors.white),
      ),
    );
  }

  /// Método para construir as informações do utilizador
  Widget _buildUserInfo() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '${user.firstName} ${user.lastName}',
          style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 4),
        Text(
          user.email,
          style: const TextStyle(fontSize: 16, color: Colors.grey),
        ),
        const SizedBox(height: 8),
        Text(
          'Género: ${user.gender == Gender.MASCULINO ? 'Masculino' : 'Feminino'}',
          style: const TextStyle(fontSize: 16),
        ),
        const SizedBox(height: 8),
        Text(
          '${user.cellPhoneNumber}',
          style: const TextStyle(fontSize: 16),
        ),
      ],
    );
  }

  /// Método para construir o botão "Histórico de Comentários"
  Widget _buildHistoryButton() {
    return ElevatedButton(
      onPressed: () {
        // Ação do botão "Histórico de Comentários"
      },
      style: ElevatedButton.styleFrom(
        backgroundColor: Colors.green,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      ),
      child: const Text(
        'Histórico de Comentários',
        style: TextStyle(color: Colors.white),
      ),
    );
  }

  /// Método para construir informações extras (anos no portal)
  Widget _buildExtraInfo() {
    return Text(
      '3 Anos no Portal de Oportunidades',
      style: const TextStyle(fontSize: 14, color: Colors.grey),
    );
  }
}
