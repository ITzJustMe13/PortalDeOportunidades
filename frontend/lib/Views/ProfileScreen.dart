import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/Views/EditProfileScreen.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/user_api_handler.dart';
import '../Models/User.dart';
import 'dart:convert';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  _ProfileScreenState createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  User? _cachedUser; // Cache for the user
  late Future<User?> _userFuture; // Declare _userFuture as late

  Future<User?> _getCachedUser() async {
    // If user is already cached, return it
    if (_cachedUser != null) {
      return _cachedUser;
    }
    // Fetch user from API and cache it
    final user = await Provider.of<UserApiHandler>(context, listen: false)
        .getStoredUser();
    _cachedUser = user;
    return user;
  }

  @override
  void initState() {
    super.initState();
    _userFuture = _getCachedUser(); // Initialize _userFuture
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: FutureBuilder<User?>(
        future: _userFuture, // Use the single Future instance
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(
              child: Text('Erro a carregar o utilizador: ${snapshot.error}'),
            );
          } else if (!snapshot.hasData) {
            return const Center(child: Text('Utilizador não foi encontrado'));
          } else {
            final user = snapshot.data!;
            return LayoutBuilder(
              builder: (context, constraints) {
                if (constraints.maxWidth < 600) {
                  return _buildMobileLayout(user);
                } else if (constraints.maxWidth < 1200) {
                  return _buildTabletLayout(user);
                } else {
                  return _buildDesktopLayout(user);
                }
              },
            );
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout(User user) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          _buildImage(user),
          const SizedBox(height: 16),
          _buildEditProfileButton(user),
          const SizedBox(height: 16),
          _buildExtraInfo(user),
          const SizedBox(height: 16),
          _buildUserInfo(user),
          const SizedBox(height: 16),
          _buildHistoryButton(),
        ],
      ),
    );
  }

  Widget _buildTabletLayout(User user) {
    return Padding(
      padding: EdgeInsets.only(left: 100.0, right: 25, top: 100),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildImage(user),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        _buildUserInfo(user),
                        _buildEditProfileButton(user),
                      ],
                    ),
                    const Divider(thickness: 1, height: 32),
                    _buildExtraInfo(user),
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

  Widget _buildDesktopLayout(User user) {
    return Padding(
      padding: const EdgeInsets.only(left: 200.0, right: 200, top: 200),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildImage(user),
              const SizedBox(height: 16),
              _buildExtraInfo(user),
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
                    _buildUserInfo(user),
                    _buildEditProfileButton(user),
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
  Widget _buildImage(User user) {
    // Check if the user image string is not null or empty
    if (user.image.isNotEmpty) {
      // Decode the Base64 string
      final decodedBytes = base64Decode(user.image);

      return Container(
        width: 150,
        height: 150,
        decoration: BoxDecoration(
          color: Colors.grey[300],
          borderRadius: BorderRadius.circular(8.0),
          image: DecorationImage(
            image: MemoryImage(decodedBytes), // Use MemoryImage for Base64
            fit: BoxFit.cover,
          ),
        ),
      );
    } else {
      // Placeholder if the user has no image
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

  /// Método para construir o botão "Editar Perfil"
  Widget _buildEditProfileButton(user) {
    return ElevatedButton(
      onPressed: () {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => EditProfileScreen(user: user),
          ),
        );
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
  Widget _buildUserInfo(User user) {
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
  Widget _buildExtraInfo(User user) {
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
