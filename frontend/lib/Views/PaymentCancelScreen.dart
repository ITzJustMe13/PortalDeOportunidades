import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';

class PaymentCancelScreen extends StatelessWidget {
  const PaymentCancelScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            // Layout for small screens (smartphones)
            return _buildMobileLayout(context);
          } else if (constraints.maxWidth < 1200) {
            // Layout for medium screens (tablets)
            return _buildTabletLayout(context);
          } else {
            // Layout for large screens (desktops)
            return _buildDesktopLayout(context);
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.check_circle,
            color: Colors.red,
            size: 100,
          ),
          const SizedBox(height: 16),
          Text(
            "Ocorreu um erro!",
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Colors.red,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            "O pagamento não foi efetuado por favor tente novamente.",
            style: TextStyle(fontSize: 16),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () {
              // Redirect to the homepage or another relevant screen
              Navigator.pushNamed(context, '/home');
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Color(0xFF50C878), // Custom green color
              padding: EdgeInsets.symmetric(horizontal: 24, vertical: 12), // Larger padding
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8), // Rounded corners
              ),
              elevation: 4, // Slight shadow for a better UI effect
            ),
            child: Text(
              "Voltar ao Início",
              style: TextStyle(
                fontSize: 16, // Adjusted font size
                fontWeight: FontWeight.bold, // Emphasize the text
                color: Colors.white, // Ensures readability on a green button
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout(BuildContext context) {
    return Center(
      child: SizedBox(
        width: 500,
        child: _buildMobileLayout(context),
      ),
    );
  }

  Widget _buildDesktopLayout(BuildContext context) {
    return Center(
      child: SizedBox(
        width: 800,
        child: Row(
          children: [
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      "Ocorreu um erro!",
                      style: TextStyle(
                        fontSize: 32,
                        fontWeight: FontWeight.bold,
                        color: Colors.red,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      "O pagamento não foi efetuado por favor tente novamente.",
                      style: TextStyle(fontSize: 18),
                    ),
                    const SizedBox(height: 24),
                    ElevatedButton(
                      onPressed: () {
                        // Redirect to the homepage or another relevant screen
                        Navigator.pushNamed(context, '/home');
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Color(0xFF50C878), // Custom green color
                        padding: EdgeInsets.symmetric(horizontal: 24, vertical: 12), // Larger padding
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8), // Rounded corners
                        ),
                        elevation: 4, // Slight shadow for a better UI effect
                      ),
                      child: Text(
                        "Voltar ao Início",
                        style: TextStyle(
                          fontSize: 16, // Adjusted font size
                          fontWeight: FontWeight.bold, // Emphasize the text
                          color: Colors.white, // Ensures readability on a green button
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
            Expanded(
              child: Icon(
                Icons.cancel,
                color: Colors.red,
                size: 150,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
