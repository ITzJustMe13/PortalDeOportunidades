import 'package:flutter/material.dart';

class CustomAppBar extends StatelessWidget implements PreferredSizeWidget {
  final List<Widget>? actions;

  const CustomAppBar({Key? key, this.actions}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        if (constraints.maxWidth < 600) {
          // Mobile layout
          return _buildMobileAppBar(context);
        } else if (constraints.maxWidth < 1200) {
          // Tablet layout
          return _buildTabletAppBar(context);
        } else {
          // Desktop layout
          return _buildDesktopAppBar(context);
        }
      },
    );
  }

  // AppBar para dispositivos móveis
  AppBar _buildMobileAppBar(BuildContext context) {
    return AppBar(
      backgroundColor: const Color(0xFF50C878),
      foregroundColor: Colors.white,
      title: Image.asset(
        'assets/images/logo.png',
        height: 50,
        width: 50,
      ),
      centerTitle: true,
      actions: [
        IconButton(
          icon: const Icon(Icons.notifications),
          onPressed: () {
            print('Notifications clicked');
          },
        ),
        IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () {
            Scaffold.of(context).openEndDrawer();
          },
        ),
      ],
    );
  }

  // AppBar para tablets
  AppBar _buildTabletAppBar(BuildContext context) {
    return AppBar(
      backgroundColor: const Color(0xFF50C878),
      foregroundColor: Colors.white,
      title: Row(
        children: [
          Image.asset(
            'assets/images/logo.png',
            height: 50,
            width: 50,
          ),
          const SizedBox(width: 8),
          const Text("Portal de Oportunidades"),
        ],
      ),
      actions: [
        IconButton(
          icon: const Icon(Icons.notifications),
          onPressed: () {
            print('Notifications clicked');
          },
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8.0),
          child: Text("exampleName"),
        ),
        IconButton(
          icon: const Icon(Icons.menu),
          onPressed: () {
            Scaffold.of(context).openEndDrawer();
          },
        ),
      ],
    );
  }

  // AppBar para desktops
  AppBar _buildDesktopAppBar(BuildContext context) {
    return AppBar(
      backgroundColor: const Color(0xFF50C878),
      foregroundColor: Colors.white,
      title: Row(
        children: [
          Image.asset(
            'assets/images/logo.png',
            height: 60,
            width: 60,
          ),
          const SizedBox(width: 16),
          const Text(
            "Portal de Oportunidades",
            style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
          ),
        ],
      ),
      actions: [
        TextButton.icon(
          onPressed: () {
            print('Notifications clicked');
          },
          icon: const Icon(Icons.notifications, color: Colors.white),
          label: const Text(
            "Notificações",
            style: TextStyle(color: Colors.white),
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8.0),
          child: Text(
            "exampleName",
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
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);
}
