import 'dart:math';

import 'package:flutter/material.dart';
import 'package:frontend/State/LoginState.dart';
import 'package:provider/provider.dart';

class CustomAppBar extends StatelessWidget implements PreferredSizeWidget {
  final List<Widget>? actions;
  final PreferredSizeWidget? bottom;

  const CustomAppBar({super.key, this.actions, this.bottom});

  @override
  Size get preferredSize {
    final double bottomHeight = bottom?.preferredSize.height ?? 0.0;
    return Size.fromHeight(kToolbarHeight + bottomHeight);
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<LoginState>(
      builder: (context, appBarState, child) {
        if (MediaQuery.of(context).size.width < 600) {
          // Mobile layout
          return _buildMobileAppBar(context, appBarState, bottom);
        } else if (MediaQuery.of(context).size.width < 1200) {
          // Tablet layout
          return _buildTabletAppBar(context, appBarState, bottom);
        } else {
          // Desktop layout
          return _buildDesktopAppBar(context, appBarState, bottom);
        }
      },
    );
  }
}

// AppBar para dispositivos móveis
AppBar _buildMobileAppBar(
    BuildContext context, LoginState appBarState, PreferredSizeWidget? bottom) {
  return AppBar(
    backgroundColor: const Color(0xFF50C878),
    foregroundColor: Colors.white,
    title: GestureDetector(
      onTap: () {
        // Handle the tap event here
        Navigator.pushNamed(context, '/'); // Example navigation
      },
      child: Image.asset(
        'assets/images/logo.png',
        height: 50,
        width: 50,
      ),
    ),
    centerTitle: true,
    actions: [
      if (!appBarState.isLoggedIn) ...[
        IconButton(
          onPressed: () {
            Navigator.pushNamed(context, '/login');
          },
          icon: const Icon(Icons.login, color: Colors.white),
        ),
        IconButton(
          onPressed: () {
            Navigator.pushNamed(context, '/register');
          },
          icon: const Icon(Icons.app_registration, color: Colors.white),
        ),
      ],
      IconButton(
        icon: const Icon(Icons.menu),
        onPressed: () {
          Scaffold.of(context).openEndDrawer();
        },
      ),
    ],
    bottom: bottom,
  );
}

// AppBar para tablets
AppBar _buildTabletAppBar(
    BuildContext context, LoginState appBarState, PreferredSizeWidget? bottom) {
  return AppBar(
    backgroundColor: const Color(0xFF50C878),
    foregroundColor: Colors.white,
    title: Row(
      children: [
        GestureDetector(
          onTap: () {
            // Handle the tap event here
            Navigator.pushNamed(context, '/'); // Example navigation
          },
          child: Image.asset(
            'assets/images/logo.png',
            height: 50,
            width: 50,
          ),
        ),
        const SizedBox(width: 8),
        const Text("Portal de Oportunidades"),
      ],
    ),
    actions: [
      if (!appBarState.isLoggedIn) ...[
        TextButton.icon(
          onPressed: () {
            Navigator.pushNamed(context, '/login');
          },
          icon: const Icon(Icons.login, color: Colors.white),
          label: const Text(
            "Login",
            style: TextStyle(color: Colors.white),
          ),
        ),
        TextButton.icon(
          onPressed: () {
            Navigator.pushNamed(context, '/register');
          },
          icon: const Icon(Icons.app_registration, color: Colors.white),
          label: const Text(
            "Registo",
            style: TextStyle(color: Colors.white),
          ),
        ),
      ],
      Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8.0),
        child: Text(
          appBarState.username,
          style: const TextStyle(fontSize: 16, color: Colors.white),
        ),
      ),
      IconButton(
        icon: const Icon(Icons.menu),
        onPressed: () {
          Scaffold.of(context).openEndDrawer();
        },
      ),
    ],
    bottom: bottom,
  );
}

// AppBar para desktops
AppBar _buildDesktopAppBar(
    BuildContext context, LoginState appBarState, PreferredSizeWidget? bottom) {
  return AppBar(
    backgroundColor: const Color(0xFF50C878),
    foregroundColor: Colors.white,
    title: Row(
      children: [
        GestureDetector(
          onTap: () {
            // Handle the tap event here
            Navigator.pushNamed(context, '/'); // Example navigation
          },
          child: Image.asset(
            'assets/images/logo.png',
            height: 60,
            width: 60,
          ),
        ),
        const SizedBox(width: 16),
        const Text(
          "Portal de Oportunidades",
          style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
        ),
      ],
    ),
    actions: [
      if (!appBarState.isLoggedIn) ...[
        TextButton.icon(
          onPressed: () {
            Navigator.pushNamed(context, '/login');
          },
          icon: const Icon(Icons.login, color: Colors.white),
          label: const Text(
            "Login",
            style: TextStyle(color: Colors.white),
          ),
        ),
        TextButton.icon(
          onPressed: () {
            Navigator.pushNamed(context, '/register');
          },
          icon: const Icon(Icons.app_registration, color: Colors.white),
          label: const Text(
            "Registo",
            style: TextStyle(color: Colors.white),
          ),
        ),
      ],
      Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8.0),
        child: Text(
          appBarState.username,
          style: const TextStyle(fontSize: 16, color: Colors.white),
        ),
      ),
      IconButton(
        icon: const Icon(Icons.menu),
        onPressed: () {
          Scaffold.of(context).openEndDrawer();
        },
      ),
    ],
    bottom: bottom,
  );
}
