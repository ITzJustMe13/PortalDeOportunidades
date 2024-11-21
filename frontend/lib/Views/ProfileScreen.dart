import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Enums/Gender.dart';
import '../Models/User.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  _ProfileScreenState createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
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
      padding: EdgeInsets.only(left: 100.0, right: 25),
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
      padding: const EdgeInsets.only(left: 200.0, right: 200, top: 200),
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
        Navigator.pushNamed(context, '/edit-profile');
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
        Navigator.pushNamed(context, '/reviews-history');
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

  /// Método para construir informações extras (anos, meses ou dias no portal)
  Widget _buildExtraInfo() {
    final now = DateTime.now();
    final duration = now.difference(user.registrationDate);

    // Calcular a diferença em anos
    final yearsOnPortal = duration.inDays ~/ 365;
    final monthsOnPortal = duration.inDays ~/ 30;
    final daysOnPortal = duration.inDays;

    String displayText;

    if (yearsOnPortal > 0) {
      displayText =
          '$yearsOnPortal ${yearsOnPortal == 1 ? "Ano" : "Anos"} no Portal de Oportunidades';
    } else if (monthsOnPortal > 0) {
      displayText =
          '$monthsOnPortal ${monthsOnPortal == 1 ? "Mês" : "Meses"} no Portal de Oportunidades';
    } else {
      displayText =
          '$daysOnPortal ${daysOnPortal == 1 ? "Dia" : "Dias"} no Portal de Oportunidades';
    }

    return Text(
      displayText,
      style: const TextStyle(fontSize: 14, color: Colors.grey),
    );
  }
}
