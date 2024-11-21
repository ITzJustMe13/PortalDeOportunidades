import 'package:flutter/material.dart';

class CustomDrawer extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          const DrawerHeader(
            decoration: BoxDecoration(
              color: Color(0xFF50C878),
            ),
            child: Text(
              'Menu',
              style: TextStyle(
                color: Colors.white,
                fontSize: 24,
              ),
            ),
          ),
          ListTile(
            leading: const Icon(Icons.home),
            title: const Text('Início'),
            onTap: () {
              Navigator.pushNamed(context, "/"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.search),
            title: const Text('Pesquisar'),
            onTap: () {
              Navigator.pushNamed(context, "/search"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.person),
            title: const Text(
              'Perfil',
            ),
            onTap: () {
              Navigator.pushNamed(context, "/profile"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.favorite),
            title: const Text('Favoritos'),
            onTap: () {
              Navigator.pushNamed(context, "/favorites"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.plus_one),
            title: const Text('Crie uma Oportunidade'),
            onTap: () {
              Navigator.pushNamed(
                  context, "/create-opportunity"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.place),
            title: const Text('As suas Oportunidades'),
            onTap: () {
              Navigator.pushNamed(
                  context, "/your-opportunities"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.book),
            title: const Text('As suas Reservas'),
            onTap: () {
              Navigator.pushNamed(
                  context, "/your-reservations"); // Close the drawer
            },
          ),
          ListTile(
            leading: const Icon(Icons.book),
            title: const Text('O seu Histórico de Reservas'),
            onTap: () {
              Navigator.pushNamed(
                  context, "/reservation-history"); // Close the drawer
            },
          ),
        ],
      ),
    );
  }
}
