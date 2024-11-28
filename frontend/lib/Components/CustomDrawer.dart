import 'package:flutter/material.dart';
import 'package:frontend/State/DrawerState.dart';
import 'package:provider/provider.dart';

class CustomDrawer extends StatefulWidget {
  const CustomDrawer({super.key});

  @override
  State<CustomDrawer> createState() => _CustomDrawerState();
}

class _CustomDrawerState extends State<CustomDrawer> {
  @override
  Widget build(BuildContext context) {
    return Consumer<CustomDrawerState>(
      builder: (context, drawerState, child) {
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
                  drawerState.navigateToRoute(context, "/"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.search),
                title: const Text('Pesquisar'),
                onTap: () {
                  drawerState.navigateToRoute(
                      context, "/search"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.person),
                title: const Text(
                  'Perfil',
                ),
                onTap: () {
                  drawerState.ensureUserLoggedIn(
                      context, "/profile"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.favorite),
                title: const Text('Favoritos'),
                onTap: () {
                  drawerState.ensureUserLoggedIn(
                      context, "/favorites"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.plus_one),
                title: const Text('Crie uma Oportunidade'),
                onTap: () {
                  drawerState.ensureUserLoggedIn(
                      context, "/create-opportunity"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.place),
                title: const Text('As suas Oportunidades'),
                onTap: () {
                  drawerState.ensureUserLoggedIn(
                      context, "/your-opportunities"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.book),
                title: const Text('As suas Reservas'),
                onTap: () {
                  drawerState.ensureUserLoggedIn(
                      context, "/your-reservations"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.book),
                title: const Text('O seu Histórico de Reservas'),
                onTap: () {
                  drawerState.ensureUserLoggedIn(
                      context, "/reservation-history"); // Close the drawer
                },
              ),
              ListTile(
                leading: const Icon(Icons.logout),
                title: const Text('Logout'),
                onTap: () {
                  drawerState.logout(context, "/"); // Close the drawer
                },
              ),
            ],
          ),
        );
      },
    );
  }
}
